using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MitsubishiLaserMES.Core.Common;
using MitsubishiLaserMES.Core.Models.Config;
using MitsubishiLaserMES.Core.Models.Eap;
using MitsubishiLaserMES.Core.Logging;
using Protocol.Core.Base;
using Protocol.Core.Messages;
using Protocol.Core.Enums;
using System.Reflection;

namespace MitsubishiLaserMES.Core.Services.Eap
{
    public class EapMqttService : IEapMqttService
    {
        private readonly MqttSettings _settings;
        private readonly OfflineBufferService _bufferService;
        private IMqttClient _mqttClient;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pendingRequests
            = new ConcurrentDictionary<string, TaskCompletionSource<string>>();

        private CancellationTokenSource _aliveCheckCts;
        private Task _aliveCheckTask;

        public bool IsConnected => _mqttClient != null && _mqttClient.IsConnected;
        public bool IsAliveGreen { get; private set; }

        public event Action<bool> ConnectionStateChanged;
        public event Action<bool> AliveStatusChanged;
        public event Action<string, string> MessageReceivedLog;
        public event Action<string, string> MessageSentLog;
        public event Action<string> LogMessage;

        public event Action<RemoteCMDMessage> RemoteCommandReceived;
        public event Action<TerminalDisplayMessage> TerminalDisplayReceived;
        public event Action<string> TimeCalibrationReceived;

        public Func<RemoteCMDMessage, Task<ReplyRemoteCMDMessage>> RemoteCommandHandler { get; set; }

        public EapMqttService(MqttSettings settings, OfflineBufferService bufferService = null)
        {
            _settings = settings ?? new MqttSettings();
            _bufferService = bufferService ?? new OfflineBufferService();
        }

        public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
        {
            if (IsConnected)
            {
                LogMessage?.Invoke("[MQTT] 目前已處於連線狀態。");
                if (!IsAliveGreen)
                {
                    LogMessage?.Invoke("[MQTT] 目前處於非存活狀態，立即觸發存活探測 (AreYouThere)...");
                    _ = SendAlivePingAsync(cancellationToken);
                }
                return true;
            }

            try
            {
                LogMessage?.Invoke($"[MQTT] 正在連線至 Broker {_settings.Server}:{_settings.Port}...");
                var factory = new MqttFactory();
                _mqttClient = factory.CreateMqttClient();

                var builder = new MqttClientOptionsBuilder()
                    .WithTcpServer(_settings.Server, _settings.Port)
                    .WithClientId(string.IsNullOrWhiteSpace(_settings.ClientId) ? Guid.NewGuid().ToString("N") : _settings.ClientId)
                    .WithCleanSession();

                if (!string.IsNullOrWhiteSpace(_settings.Username))
                {
                    builder.WithCredentials(_settings.Username, _settings.Password);
                }

                _mqttClient.ApplicationMessageReceivedAsync += HandleIncomingMessageAsync;
                _mqttClient.DisconnectedAsync += HandleDisconnectedAsync;

                var connectResult = await _mqttClient.ConnectAsync(builder.Build(), cancellationToken).ConfigureAwait(false);
                if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
                {
                    LogMessage?.Invoke($"[MQTT] 連線成功！正在訂閱指令主題: {_settings.CommandTopic}");
                    await _mqttClient.SubscribeAsync(_settings.CommandTopic, MqttQualityOfServiceLevel.AtLeastOnce, cancellationToken).ConfigureAwait(false);

                    ConnectionStateChanged?.Invoke(true);

                    // 啟動存活檢測
                    StartAliveCheck();

                    // 補發離線暫存
                    _ = FlushOfflineBufferAsync();

                    return true;
                }
                else
                {
                    LogMessage?.Invoke($"[MQTT] 連線未成功: {connectResult.ReasonString}");
                    ConnectionStateChanged?.Invoke(false);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"[MQTT 連線失敗] {ex.Message}");
                ConnectionStateChanged?.Invoke(false);
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            StopAliveCheck();
            if (_mqttClient != null)
            {
                try
                {
                    if (_mqttClient.IsConnected)
                    {
                        await _mqttClient.DisconnectAsync().ConfigureAwait(false);
                    }
                }
                catch { }
                finally
                {
                    _mqttClient.Dispose();
                    _mqttClient = null;
                }
            }
            IsAliveGreen = false;
            ConnectionStateChanged?.Invoke(false);
            AliveStatusChanged?.Invoke(false);
            LogMessage?.Invoke("[MQTT] 已斷開連線。");
        }

        public async Task<TReply> SendRequestAsync<TReq, TReply>(TReq reqPayload, CancellationToken cancellationToken = default)
            where TReq : EapPayloadBase
            where TReply : EapReplyPayloadBase, new()
        {
            if (string.IsNullOrWhiteSpace(reqPayload.TransactionID))
            {
                reqPayload.TransactionID = Guid.NewGuid().ToString();
            }
            if (string.IsNullOrWhiteSpace(reqPayload.Machine))
            {
                reqPayload.Machine = _settings.EqID;
            }
            reqPayload.DATE = DateTimeUtils.NowEapDate();

            var envelope = new EapEnvelope<TReq>(reqPayload);
            var json = JsonConvert.SerializeObject(envelope, Formatting.None);

            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            _pendingRequests[reqPayload.TransactionID] = tcs;

            try
            {
                if (!IsConnected)
                {
                    throw new InvalidOperationException("EAP MQTT 尚未連線，無法發送請求。");
                }

                await PublishRawAsync(_settings.ReportTopic, json, cancellationToken).ConfigureAwait(false);

                // 設定 T1 逾時時間 (預設 45 秒)
                int timeoutMs = _settings.TimeoutT1Ms > 0 ? _settings.TimeoutT1Ms : 45000;
                using var timeoutCts = new CancellationTokenSource(timeoutMs);
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                using (linkedCts.Token.Register(() => tcs.TrySetCanceled()))
                {
                    string replyJson = await tcs.Task.ConfigureAwait(false);
                    return ParseReplyPayload<TReply>(replyJson);
                }
            }
            catch (OperationCanceledException)
            {
                int timeoutSec = (_settings.TimeoutT1Ms > 0 ? _settings.TimeoutT1Ms : 45000) / 1000;
                LogMessage?.Invoke($"[EAP 逾時] CMD: {reqPayload.CMD} 等待 EAP 回覆超過 {timeoutSec}s 逾時。");
                return new TReply
                {
                    TransactionID = reqPayload.TransactionID,
                    RtnResult = "FAIL",
                    RtnMsg = "System No Response (Timeout)"
                };
            }
            finally
            {
                _pendingRequests.TryRemove(reqPayload.TransactionID, out _);
            }
        }

        public async Task<bool> PublishReportAsync<T>(T payload, bool bufferIfOffline = true)
            where T : EapPayloadBase
        {
            if (string.IsNullOrWhiteSpace(payload.TransactionID))
            {
                payload.TransactionID = Guid.NewGuid().ToString();
            }
            if (string.IsNullOrWhiteSpace(payload.Machine))
            {
                payload.Machine = _settings.EqID;
            }
            payload.DATE = DateTimeUtils.NowEapDate();

            var envelope = new EapEnvelope<T>(payload);
            var json = JsonConvert.SerializeObject(envelope, Formatting.None);

            if (!IsConnected)
            {
                if (bufferIfOffline)
                {
                    LogMessage?.Invoke($"[MQTT 離線] 訊息存入暫存佇列 (CMD: {payload.CMD})");
                    _bufferService.Enqueue(_settings.ReportTopic, json);
                }
                return false;
            }

            try
            {
                await PublishRawAsync(_settings.ReportTopic, json).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"[MQTT 發送異常] {ex.Message}");
                if (bufferIfOffline)
                {
                    _bufferService.Enqueue(_settings.ReportTopic, json);
                }
                return false;
            }
        }

        public async Task<TReply> SendProtocolRequestAsync<TReq, TReply>(TReq reqPayload, CancellationToken cancellationToken = default)
            where TReq : BaseMessage
            where TReply : ReplyBase, new()
        {
            var mapping = reqPayload.GetType().GetCustomAttribute<CommandMappingAttribute>();
            if (mapping != null)
            {
                reqPayload.CMD = mapping.Command;
            }

            if (string.IsNullOrWhiteSpace(reqPayload.TransactionID))
            {
                reqPayload.TransactionID = Guid.NewGuid().ToString();
            }
            if (string.IsNullOrWhiteSpace(reqPayload.Machine))
            {
                reqPayload.Machine = _settings.EqID;
            }
            if (string.IsNullOrWhiteSpace(reqPayload.Date))
            {
                reqPayload.Date = DateTimeUtils.NowEapDate();
            }

            var envelope = new EapEnvelope<TReq>(reqPayload);
            var json = JsonConvert.SerializeObject(envelope, Formatting.None);

            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            _pendingRequests[reqPayload.TransactionID] = tcs;

            try
            {
                if (!IsConnected)
                {
                    throw new InvalidOperationException("EAP MQTT 尚未連線，無法發送請求。");
                }

                await PublishRawAsync(_settings.ReportTopic, json, cancellationToken).ConfigureAwait(false);

                // 設定 T1 逾時時間 (預設 45 秒)
                int timeoutMs = _settings.TimeoutT1Ms > 0 ? _settings.TimeoutT1Ms : 45000;
                using var timeoutCts = new CancellationTokenSource(timeoutMs);
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                using (linkedCts.Token.Register(() => tcs.TrySetCanceled()))
                {
                    string replyJson = await tcs.Task.ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<TReply>(replyJson) ?? new TReply();
                }
            }
            catch (OperationCanceledException)
            {
                int timeoutSec = (_settings.TimeoutT1Ms > 0 ? _settings.TimeoutT1Ms : 45000) / 1000;
                LogMessage?.Invoke($"[EAP 逾時] CMD: {reqPayload.CMD} 等待 EAP 回覆超過 {timeoutSec}s 逾時。");
                return new TReply
                {
                    TransactionID = reqPayload.TransactionID,
                    RtnResult = RtnResult.FAIL,
                    RtnMsg = "System No Response (Timeout)"
                };
            }
            finally
            {
                _pendingRequests.TryRemove(reqPayload.TransactionID, out _);
            }
        }

        public async Task<bool> PublishProtocolReportAsync<T>(T payload, bool bufferIfOffline = true)
            where T : BaseMessage
        {
            var mapping = payload.GetType().GetCustomAttribute<CommandMappingAttribute>();
            if (mapping != null)
            {
                payload.CMD = mapping.Command;
            }

            if (string.IsNullOrWhiteSpace(payload.TransactionID))
            {
                payload.TransactionID = Guid.NewGuid().ToString();
            }
            if (string.IsNullOrWhiteSpace(payload.Machine))
            {
                payload.Machine = _settings.EqID;
            }
            if (string.IsNullOrWhiteSpace(payload.Date))
            {
                payload.Date = DateTimeUtils.NowEapDate();
            }

            var envelope = new EapEnvelope<T>(payload);
            var json = JsonConvert.SerializeObject(envelope, Formatting.None);

            if (!IsConnected)
            {
                if (bufferIfOffline)
                {
                    LogMessage?.Invoke($"[MQTT 離線] 訊息存入暫存佇列 (CMD: {payload.CMD})");
                    _bufferService.Enqueue(_settings.ReportTopic, json);
                }
                return false;
            }

            try
            {
                await PublishRawAsync(_settings.ReportTopic, json).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"[MQTT 發送異常] {ex.Message}");
                if (bufferIfOffline)
                {
                    _bufferService.Enqueue(_settings.ReportTopic, json);
                }
                return false;
            }
        }

        private async Task PublishRawAsync(string topic, string json, CancellationToken cancellationToken = default)
        {
            var msg = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(json)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await _mqttClient.PublishAsync(msg, cancellationToken).ConfigureAwait(false);
            MessageSentLog?.Invoke(topic, json);
            LogService.Instance.Debug("MQTT-TX", $"[Topic: {topic}] {json}", _settings.EqID);
        }

        private Task HandleIncomingMessageAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            string topic = e.ApplicationMessage.Topic;
            string payloadStr = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            MessageReceivedLog?.Invoke(topic, payloadStr);
            LogService.Instance.Debug("MQTT-RX", $"[Topic: {topic}] {payloadStr}", _settings.EqID);

            try
            {
                JObject jObj = JObject.Parse(payloadStr);
                JObject payloadObj = jObj["Payload"] as JObject ?? jObj;

                string cmd = payloadObj["CMD"]?.ToString()?.Trim() ?? string.Empty;
                string transactionId = payloadObj["TransactionID"]?.ToString()?.Trim() ?? string.Empty;

                // 1. 若為存活檢測回覆 (IamHere)，代表雙向鏈路暢通，立即點亮綠燈 (AREYOUTHERE 與 IAMHERE 不需要判斷 RtnResult)
                if (string.Equals(cmd, "IamHere", StringComparison.OrdinalIgnoreCase))
                {
                    IsAliveGreen = true;
                    AliveStatusChanged?.Invoke(true);
                }

                // 2. 若明確為 Reply 類別且為等待中的請求 (避免將共用 TransactionID 之 RemoteCMD 等指令誤判為 Reply)
                bool isReplyMessage = cmd.StartsWith("Reply", StringComparison.OrdinalIgnoreCase)
                                   || string.Equals(cmd, "IamHere", StringComparison.OrdinalIgnoreCase);

                if (isReplyMessage && !string.IsNullOrEmpty(transactionId) && _pendingRequests.TryGetValue(transactionId, out var tcs))
                {
                    tcs.TrySetResult(payloadObj.ToString());
                    return Task.CompletedTask;
                }

                // 3. BC 發起的 AreYouThere -> EQ 回覆 IamHere
                if (string.Equals(cmd, "AreYouThere", StringComparison.OrdinalIgnoreCase))
                {
                    IsAliveGreen = true;
                    AliveStatusChanged?.Invoke(true);

                    var reply = new IamHereMessage
                    {
                        TransactionID = transactionId,
                        Machine = _settings.EqID,
                        Date = DateTimeUtils.NowEapDate()
                    };
                    _ = PublishProtocolReportAsync(reply, false);
                    return Task.CompletedTask;
                }

                // 4. BC 時間校正 TimeCalibrate
                if (string.Equals(cmd, "TimeCalibrate", StringComparison.OrdinalIgnoreCase))
                {
                    string serverDate = payloadObj["Date"]?.ToString() ?? payloadObj["DATE"]?.ToString() ?? DateTimeUtils.NowEapDate();
                    TimeCalibrationReceived?.Invoke(serverDate);

                    var reply = new ReplyTimeCalibrateMessage
                    {
                        TransactionID = transactionId,
                        Machine = _settings.EqID,
                        Date = DateTimeUtils.NowEapDate(),
                        RtnResult = RtnResult.PASS
                    };
                    _ = PublishProtocolReportAsync(reply, false);
                    return Task.CompletedTask;
                }

                // 5. BC 遠端指令 RemoteCMD (支援交握雷射機配方後非同步回覆)
                if (string.Equals(cmd, "RemoteCMD", StringComparison.OrdinalIgnoreCase))
                {
                    RemoteCMDMessage remoteCmd = null;
                    try
                    {
                        remoteCmd = JsonConvert.DeserializeObject<RemoteCMDMessage>(payloadObj.ToString());
                    }
                    catch { }

                    if (remoteCmd == null)
                    {
                        remoteCmd = new RemoteCMDMessage
                        {
                            TransactionID = transactionId,
                            Machine = _settings.EqID,
                            Date = DateTimeUtils.NowEapDate()
                        };
                    }

                    RemoteCommandReceived?.Invoke(remoteCmd);

                    if (RemoteCommandHandler != null)
                    {
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                var reply = await RemoteCommandHandler(remoteCmd).ConfigureAwait(false);
                                if (reply != null)
                                {
                                    if (string.IsNullOrWhiteSpace(reply.TransactionID)) reply.TransactionID = transactionId;
                                    if (string.IsNullOrWhiteSpace(reply.Machine)) reply.Machine = _settings.EqID;
                                    if (string.IsNullOrWhiteSpace(reply.Date)) reply.Date = DateTimeUtils.NowEapDate();
                                    await PublishProtocolReportAsync(reply, false).ConfigureAwait(false);
                                }
                            }
                            catch (Exception ex)
                            {
                                LogMessage?.Invoke($"[RemoteCMD 處理異常] {ex.Message}");
                            }
                        });
                    }
                    else
                    {
                        var reply = new ReplyRemoteCMDMessage
                        {
                            TransactionID = transactionId,
                            Machine = _settings.EqID,
                            Date = DateTimeUtils.NowEapDate(),
                            RemoteCMDType = remoteCmd.RemoteCMDType,
                            RtnResult = RtnResult.PASS,
                            RtnMsg = $"[{remoteCmd.RemoteCMDType}] executed successfully"
                        };
                        _ = PublishProtocolReportAsync(reply, false);
                    }
                    return Task.CompletedTask;
                }

                // 6. BC 遠端訊息 TerminalDisplay
                if (string.Equals(cmd, "TerminalDisplay", StringComparison.OrdinalIgnoreCase))
                {
                    TerminalDisplayMessage termMsg = null;
                    try
                    {
                        termMsg = JsonConvert.DeserializeObject<TerminalDisplayMessage>(payloadObj.ToString());
                    }
                    catch { }

                    if (termMsg != null)
                    {
                        TerminalDisplayReceived?.Invoke(termMsg);
                    }

                    var reply = new ReplyTerminalDisplayMessage
                    {
                        TransactionID = transactionId,
                        Machine = _settings.EqID,
                        Date = DateTimeUtils.NowEapDate(),
                        RtnResult = RtnResult.PASS
                    };
                    _ = PublishProtocolReportAsync(reply, false);
                    return Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"[MQTT 處理訊息失敗] {ex.Message}");
            }

            return Task.CompletedTask;
        }

        private Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs e)
        {
            IsAliveGreen = false;
            ConnectionStateChanged?.Invoke(false);
            AliveStatusChanged?.Invoke(false);
            LogMessage?.Invoke($"[MQTT] 已自 Broker 斷線: {e.Reason}");

            // 自動重連
            _ = Task.Run(async () =>
            {
                await Task.Delay(5000);
                if (_mqttClient != null && !_mqttClient.IsConnected)
                {
                    LogMessage?.Invoke("[MQTT] 嘗試重新連線中...");
                    try
                    {
                        await ConnectAsync().ConfigureAwait(false);
                    }
                    catch { }
                }
            });

            return Task.CompletedTask;
        }

        private void StartAliveCheck()
        {
            StopAliveCheck();
            _aliveCheckCts = new CancellationTokenSource();
            _aliveCheckTask = Task.Run(() => AliveCheckLoopAsync(_aliveCheckCts.Token));
        }

        private void StopAliveCheck()
        {
            _aliveCheckCts?.Cancel();
        }

        private async Task AliveCheckLoopAsync(CancellationToken token)
        {
            // 連線建立後先立即進行一次雙向心跳探測
            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(500, token).ConfigureAwait(false);
                    await SendAlivePingAsync(token).ConfigureAwait(false);
                }
                catch { }
            }, token);

            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_settings.AliveCheckIntervalSec), token).ConfigureAwait(false);
                    await SendAlivePingAsync(token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    IsAliveGreen = false;
                    AliveStatusChanged?.Invoke(false);
                }
            }
        }

        private async Task SendAlivePingAsync(CancellationToken token)
        {
            if (!IsConnected) return;

            try
            {
                var ping = new AliveCheckReqPayload
                {
                    Machine = _settings.EqID
                };
                var reply = await SendRequestAsync<AliveCheckReqPayload, AliveCheckReplyPayload>(ping, token).ConfigureAwait(false);
                // AREYOUTHERE 與 IAMHERE 不需要判斷 RtnResult，只要收到回覆且 CMD 為 IamHere 即代表存活
                bool ok = reply != null && string.Equals(reply.CMD, "IamHere", StringComparison.OrdinalIgnoreCase);
                IsAliveGreen = ok;
                AliveStatusChanged?.Invoke(ok);
            }
            catch
            {
                IsAliveGreen = false;
                AliveStatusChanged?.Invoke(false);
            }
        }

        private async Task FlushOfflineBufferAsync()
        {
            if (_bufferService.Count == 0) return;
            LogMessage?.Invoke($"[MQTT 暫存補傳] 開始補發 {_bufferService.Count} 筆離線訊息...");
            while (IsConnected && _bufferService.TryDequeue(out var item))
            {
                try
                {
                    await PublishRawAsync(item.Topic, item.JsonContent).ConfigureAwait(false);
                    await Task.Delay(100);
                }
                catch
                {
                    _bufferService.Enqueue(item.Topic, item.JsonContent);
                    break;
                }
            }
            LogMessage?.Invoke("[MQTT 暫存補傳] 補發完畢。");
        }

        private TReply ParseReplyPayload<TReply>(string json) where TReply : EapReplyPayloadBase, new()
        {
            try
            {
                return JsonConvert.DeserializeObject<TReply>(json) ?? new TReply();
            }
            catch
            {
                return new TReply
                {
                    RtnResult = "FAIL",
                    RtnMsg = "回覆內容解析失敗: " + json
                };
            }
        }

        public void Dispose()
        {
            DisconnectAsync().GetAwaiter().GetResult();
            _aliveCheckCts?.Dispose();
        }
    }
}
