using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MitsubishiLaserMES.Core.Common;
using MitsubishiLaserMES.Core.Logging;
using MitsubishiLaserMES.Core.Models.Config;
using MitsubishiLaserMES.Core.Models.Eap;
using MitsubishiLaserMES.Core.Services.Eap;
using MitsubishiLaserMES.Core.Services.Opc;
using Protocol.Core.Base;
using Protocol.Core.Messages;
using Protocol.Core.Enums;

namespace MitsubishiLaserMES.Core.Services.Coordination
{
    public class MesCoordinator : IMesCoordinator
    {
        private readonly AppConfig _config;
        private readonly ILogService _logger;
        private readonly TrackedOrderStorageService _storageService;
        private readonly List<TrackedInOrderInfo> _trackedInOrders = new List<TrackedInOrderInfo>();

        public IEapMqttService EapService { get; }
        public IOpcService OpcService { get; }

        public string CurrentOperatorId { get; private set; } = string.Empty;
        public string CurrentOperatorName { get; private set; } = string.Empty;
        public bool IsOperatorLoggedIn => !string.IsNullOrWhiteSpace(CurrentOperatorId);
        public bool IsTrackedIn => CurrentOrder != null;
        public TrackedInOrderInfo CurrentOrder { get; private set; }
        public IReadOnlyList<TrackedInOrderInfo> TrackedInOrders => _trackedInOrders.AsReadOnly();

        public event Action<string, string> OperatorLoggedIn;
        public event Action OperatorLoggedOut;
        public event Action<TrackedInOrderInfo> TrackInCompleted;
        public event Action<string> TrackOutCompleted;
        public event Action<string> TrackInRemoved;
        public event Action<string> TerminalMessageNotified;
        public event Action<string> SystemLogMessage;

        public MesCoordinator(AppConfig config, IEapMqttService eapService, IOpcService opcService, ILogService logger = null, TrackedOrderStorageService storageService = null)
        {
            _config = config ?? new AppConfig();
            _logger = logger ?? LogService.Instance;
            _storageService = storageService ?? new TrackedOrderStorageService();
            if (!string.IsNullOrWhiteSpace(_config.Mqtt?.EqID))
            {
                _logger.DefaultEquipmentId = _config.Mqtt.EqID;
            }

            EapService = eapService;
            OpcService = opcService;

            // 自動從本機還原已進站工單快取 (重啟不遺失)
            try
            {
                var persisted = _storageService.Load();
                if (persisted != null && persisted.Count > 0)
                {
                    _trackedInOrders.AddRange(persisted);
                    CurrentOrder = _trackedInOrders.LastOrDefault();
                    _logger.Info("Coordinator", $"[本地快取還原] 已自動載入 {persisted.Count} 筆未出站工單 (當前主工單: {CurrentOrder?.WorkOrder})");
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Coordinator", $"[本地快取還原失敗] {ex.Message}");
            }

            // 綁定日誌
            EapService.LogMessage += msg =>
            {
                _logger.Info("MQTT", msg, _config.Mqtt?.EqID);
                SystemLogMessage?.Invoke(msg);
            };
            OpcService.LogMessage += msg =>
            {
                _logger.Info("OPC", msg, _config.Mqtt?.EqID);
                SystemLogMessage?.Invoke(msg);
            };

            // 綁定 OPC 狀態與事件監聽
            OpcService.StatusLightChanged += OnOpcStatusLightChanged;
            OpcService.ProcessedCountChanged += OnOpcProcessedCountChanged;
            OpcService.AlarmTriggered += OnOpcAlarmTriggered;
            OpcService.ConnectionStateChanged += OnOpcConnectionStateChanged;

            // 綁定 EAP 下行指令與遠端指令交握處理器
            EapService.RemoteCommandHandler = HandleRemoteCommandAsync;
            EapService.RemoteCommandReceived += OnRemoteCommandReceived;
            EapService.TerminalDisplayReceived += OnTerminalDisplayReceived;
            EapService.TimeCalibrationReceived += OnTimeCalibrationReceived;
        }

        public async Task<bool> InitializeAsync(CancellationToken cancellationToken = default)
        {
            SystemLogMessage?.Invoke("[系統初始化] 開始連線上位 EAP 與機台 OPC...");
            var eapTask = EapService.ConnectAsync(cancellationToken);
            var opcTask = OpcService.ConnectAsync(cancellationToken);
            await Task.WhenAll(eapTask, opcTask).ConfigureAwait(false);

            SystemLogMessage?.Invoke($"[系統初始化完成] EAP連線: {EapService.IsConnected}, OPC連線: {OpcService.IsConnected}");

            // 程式啟動初始化完成後，若機台 OPC 已連線，自動下令切換機台為 SEMI-AUTO 模式
            if (OpcService.IsConnected && OpcService.CurrentOpcMode != MitsubishiOpcMode.OnlineSemiAuto)
            {
                try
                {
                    SystemLogMessage?.Invoke("[模式切換] 程式啟動連線完成，自動下令機台切換為 SEMI-AUTO (OnlineSemiAuto) 模式...");
                    bool modeOk = await SwitchOpcModeAsync((short)MitsubishiOpcMode.OnlineSemiAuto, cancellationToken).ConfigureAwait(false);
                    if (modeOk)
                    {
                        _logger.Info("Coordinator", "程式啟動完成，機台已自動切換為 SEMI-AUTO 模式。");
                        SystemLogMessage?.Invoke("[模式切換成功] 機台已切換為 SEMI-AUTO 模式。");
                    }
                    else
                    {
                        _logger.Warn("Coordinator", "程式啟動完成，但自動切換為 SEMI-AUTO 模式未成功。");
                        SystemLogMessage?.Invoke("[模式切換未成功] 自動切換為 SEMI-AUTO 模式未成功 (機台可能非 Idle 狀態)。");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warn("Coordinator", $"[模式切換例外] {ex.Message}");
                }
            }

            return EapService.IsConnected || OpcService.IsConnected;
        }

        public async Task<UserVerifyReplyPayload> LoginWithBarcodeAsync(string userBarcode, CancellationToken cancellationToken = default)
        {
            if (!EapService.IsAliveGreen)
            {
                _logger.Warn("UserVerify", "EAP 未連線 (非綠燈狀態)，無法進行人員登入。");
                SystemLogMessage?.Invoke("[人員驗證失敗] EAP 未連線。");
                return new UserVerifyReplyPayload
                {
                    RtnResult = "FAIL",
                    RtnMsg = "EAP未連線"
                };
            }

            if (string.IsNullOrWhiteSpace(userBarcode))
            {
                return new UserVerifyReplyPayload { RtnResult = "FAIL", RtnMsg = "請輸入或掃描人員二維條碼" };
            }

            string eventId = _config.Profile?.GetEventId("USER_VERIFY", "104") ?? "104";
            string eventName = _config.Profile?.GetEventName(eventId, "USER_VERIFY") ?? "USER_VERIFY";
            string barcodeParam = _config.Profile?.GetVariableId("User_Barcode", "1028") ?? "1028";

            _logger.Info("UserVerify", $"正在向 EAP 驗證工號/條碼: {userBarcode} (EventID: {eventId}, Param: {barcodeParam})");
            SystemLogMessage?.Invoke($"[人員驗證] 正在向 EAP 驗證工號/條碼: {userBarcode} (EventID: {eventId})");
            var req = new UserVerifyReqPayload(eventId, eventName)
            {
                Machine = _config.Mqtt.EqID,
                Data = new List<EventReportItem>
                {
                    new EventReportItem { Parameter = barcodeParam, Value = userBarcode }
                }
            };

            var reply = await EapService.SendRequestAsync<UserVerifyReqPayload, UserVerifyReplyPayload>(req, cancellationToken).ConfigureAwait(false);
            if (reply.IsPass)
            {
                // EAP 回傳的 rtnmessage 即為人員工號，直接擷取
                string employeeId = !string.IsNullOrWhiteSpace(reply.EmployeeId) ? reply.EmployeeId : userBarcode.Trim();
                CurrentOperatorId = employeeId;
                CurrentOperatorName = employeeId;
                OperatorLoggedIn?.Invoke(CurrentOperatorId, CurrentOperatorName);
                _logger.Info("UserVerify", $"人員登入成功，工號: {CurrentOperatorId}");
                SystemLogMessage?.Invoke($"[人員登入成功] 工號: {CurrentOperatorId}");
            }
            else
            {
                _logger.Warn("UserVerify", $"人員登入失敗: {reply.RtnMsg}");
                SystemLogMessage?.Invoke($"[人員登入失敗] {reply.RtnMsg}");
            }
            return reply;
        }

        public void LogoutOperator()
        {
            _logger.Info("UserLogout", $"人員登出: 工號 {CurrentOperatorId}");
            CurrentOperatorId = string.Empty;
            CurrentOperatorName = string.Empty;
            OperatorLoggedOut?.Invoke();
            SystemLogMessage?.Invoke("[人員登出] 目前已切換為未登入狀態。");
        }

        public async Task<ReplyTrackInReqMessage> TrackInAsync(TrackInReqMessage req, CancellationToken cancellationToken = default)
        {
            if (!EapService.IsAliveGreen)
            {
                _logger.Warn("TrackIn", "EAP 未連線 (非綠燈狀態)，無法進行工單進站。");
                SystemLogMessage?.Invoke("[工單進站失敗] EAP 未連線。");
                return new ReplyTrackInReqMessage
                {
                    CMD = CommandType.ReplyTrackInReq,
                    RtnResult = RtnResult.FAIL,
                    RtnMsg = "EAP未連線"
                };
            }

            if (string.IsNullOrWhiteSpace(req.UserID))
            {
                req.UserID = CurrentOperatorId;
            }
            req.CMD = CommandType.TrackInReq;
            req.Machine = _config.Mqtt?.EqID ?? string.Empty;
            if (string.IsNullOrWhiteSpace(req.Date))
            {
                req.Date = DateTimeUtils.NowEapDate();
            }
            if (string.IsNullOrWhiteSpace(req.TransactionID))
            {
                req.TransactionID = Guid.NewGuid().ToString();
            }

            string woStr = req.WorkOrder != null && req.WorkOrder.Count > 0 ? string.Join(",", req.WorkOrder) : string.Empty;
            string cstStr = req.CassetteID != null && req.CassetteID.Count > 0 ? string.Join(",", req.CassetteID) : string.Empty;

            _logger.Info("TrackIn", $"發送進站請求: 工單={woStr}, 卡匣={cstStr}, 數量={req.Qty}, 操作員={req.UserID}");
            SystemLogMessage?.Invoke($"[工單進站] 發送 TrackInReq: 工單={woStr}, 卡匣={cstStr}, 數量={req.Qty}");

            var reply = await EapService.SendProtocolRequestAsync<TrackInReqMessage, ReplyTrackInReqMessage>(req, cancellationToken).ConfigureAwait(false);

            bool isTrackInSuccess = (reply.RtnResult == RtnResult.PASS || string.Equals(reply.RtnResult.ToString(), "SUCCESS", StringComparison.OrdinalIgnoreCase))
                && (reply.CMD == CommandType.ReplyTrackInReq || reply.CMD == CommandType.AlarmReport);
            if (isTrackInSuccess)
            {
                string primaryWo = reply.WorkOrder != null && reply.WorkOrder.Count > 0 ? reply.WorkOrder[0] : (req.WorkOrder != null && req.WorkOrder.Count > 0 ? req.WorkOrder[0] : "");
                string primaryCst = reply.CassetteID != null && reply.CassetteID.Count > 0 ? reply.CassetteID[0] : (req.CassetteID != null && req.CassetteID.Count > 0 ? req.CassetteID[0] : "");
                short qty = 0;
                if (!string.IsNullOrWhiteSpace(req.Qty))
                {
                    short.TryParse(req.Qty, out qty);
                }

                var orderInfo = new TrackedInOrderInfo
                {
                    WorkOrder = primaryWo,
                    CassetteId = primaryCst,
                    RecipeId = !string.IsNullOrWhiteSpace(OpcService.ActiveProgramFile) ? OpcService.ActiveProgramFile : (CurrentOrder?.RecipeId ?? string.Empty),
                    PartNo = req.MaterialID ?? string.Empty,
                    TotalQty = qty,
                    TrackInTime = DateTime.Now
                };

                CurrentOrder = orderInfo;
                _trackedInOrders.Add(orderInfo);
                _storageService.Save(_trackedInOrders);
                TrackInCompleted?.Invoke(orderInfo);

                _logger.Info("TrackIn", $"工單進站成功: 工單={primaryWo}, 卡匣={primaryCst}");
                SystemLogMessage?.Invoke($"[工單進站成功] 工單={primaryWo}, 卡匣={primaryCst}");

                // 進站成功後依機台 OPC 模式決定是否自動啟動連續加工運轉
                if (OpcService.IsConnected)
                {
                    if (OpcService.CurrentOpcMode == MitsubishiOpcMode.OnlineAuto)
                    {
                        _logger.Info("TrackIn", $"目前機台為全自動模式 (OnlineAuto)，進站成功後自動啟動連續加工運轉。");
                        SystemLogMessage?.Invoke($"[工單進站成功] 機台為全自動模式 (Auto)，準備自動下發啟動運轉命令...");
                        _ = Task.Run(async () =>
                        {
                            await Task.Delay(300).ConfigureAwait(false);
                            await OpcService.StartScheduleAsync().ConfigureAwait(false);
                        });
                    }
                    else if (OpcService.CurrentOpcMode == MitsubishiOpcMode.OnlineSemiAuto)
                    {
                        _logger.Info("TrackIn", $"目前機台為半自動模式 (OnlineSemiAuto)，進站成功後不自動啟動，等待作業員於機台端按下啟動按鈕。");
                        SystemLogMessage?.Invoke($"[工單進站成功] 目前為半自動模式 (Semi-Auto)，配方已就緒，請作業員於機台端手動按下啟動按鈕。");
                    }
                    else
                    {
                        _logger.Info("TrackIn", $"目前機台 OPC 模式為 {OpcService.CurrentOpcMode}，不自動發送啟動命令。");
                        SystemLogMessage?.Invoke($"[工單進站成功] 目前機台模式為 {OpcService.CurrentOpcMode}，不自動啟動加工。");
                    }
                }
            }
            else
            {
                _logger.Warn("TrackIn", $"工單進站失敗: {reply.RtnMsg}");
                SystemLogMessage?.Invoke($"[工單進站失敗] 原因: {reply.RtnMsg}");
            }

            return reply;
        }

        public async Task<ReplyTrackInReqPayload> TrackInAsync(TrackInReqPayload req, CancellationToken cancellationToken = default)
        {
            var msg = new TrackInReqMessage
            {
                WorkOrder = !string.IsNullOrWhiteSpace(req.WorkOrder) ? new List<string> { req.WorkOrder } : new List<string>(),
                CassetteID = !string.IsNullOrWhiteSpace(req.CassetteID) ? new List<string> { req.CassetteID } : new List<string>(),
                MaterialID = req.MaterialID,
                UserID = req.UserID,
                ToolingID = req.ToolingID,
                PanelID = req.PanelID,
                Qty = req.Qty
            };

            var replyMsg = await TrackInAsync(msg, cancellationToken).ConfigureAwait(false);
            return new ReplyTrackInReqPayload
            {
                TransactionID = replyMsg.TransactionID,
                Machine = replyMsg.Machine,
                RtnResult = replyMsg.RtnResult.ToString(),
                RtnMsg = replyMsg.RtnMsg,
                WorkOrder = replyMsg.WorkOrder ?? new List<string>(),
                CassetteID = replyMsg.CassetteID ?? new List<string>(),
                UserID = replyMsg.UserID,
                MaterialID = replyMsg.MaterialID,
                ToolingID = replyMsg.ToolingID
            };
        }

        public async Task<ReplyTrackOutReqMessage> TrackOutAsync(TrackOutReqMessage req, CancellationToken cancellationToken = default)
        {
            if (!EapService.IsAliveGreen)
            {
                _logger.Warn("TrackOut", "EAP 未連線 (非綠燈狀態)，無法進行工單出站。");
                SystemLogMessage?.Invoke("[工單出站失敗] EAP 未連線。");
                return new ReplyTrackOutReqMessage
                {
                    CMD = CommandType.ReplyTrackOutReq,
                    RtnResult = RtnResult.FAIL,
                    RtnMsg = "EAP未連線"
                };
            }

            if (string.IsNullOrWhiteSpace(req.UserID))
            {
                req.UserID = CurrentOperatorId;
            }
            req.CMD = CommandType.TrackOutReq;
            req.Machine = _config.Mqtt?.EqID ?? string.Empty;
            if (string.IsNullOrWhiteSpace(req.Date))
            {
                req.Date = DateTimeUtils.NowEapDate();
            }
            if (string.IsNullOrWhiteSpace(req.TransactionID))
            {
                req.TransactionID = Guid.NewGuid().ToString();
            }

            string wo = req.WorkOrder != null && req.WorkOrder.Count > 0 ? string.Join(",", req.WorkOrder) : (CurrentOrder?.WorkOrder ?? string.Empty);
            _logger.Info("TrackOut", $"發送出站請求: 工單={wo}, 數量={req.Qty}, 結果={req.Result}, NGCode={req.NGCode}");
            SystemLogMessage?.Invoke($"[工單出站] 發送 TrackOutReq: 工單={wo}, 數量={req.Qty}, 結果={req.Result}");

            var reply = await EapService.SendProtocolRequestAsync<TrackOutReqMessage, ReplyTrackOutReqMessage>(req, cancellationToken).ConfigureAwait(false);

            bool isTrackOutSuccess = (reply.RtnResult == RtnResult.PASS || string.Equals(reply.RtnResult.ToString(), "SUCCESS", StringComparison.OrdinalIgnoreCase))
                && (reply.CMD == CommandType.ReplyTrackOutReq || reply.CMD == CommandType.AlarmReport);
            if (isTrackOutSuccess)
            {
                string primaryWo = req.WorkOrder?.FirstOrDefault() ?? (CurrentOrder?.WorkOrder ?? "");
                _trackedInOrders.RemoveAll(o => o.WorkOrder == primaryWo);
                if (CurrentOrder?.WorkOrder == primaryWo)
                {
                    CurrentOrder = _trackedInOrders.LastOrDefault();
                }
                _storageService.Save(_trackedInOrders);
                TrackOutCompleted?.Invoke(primaryWo);
                _logger.Info("TrackOut", $"工單出站成功: 工單 {primaryWo} 帳務過帳完成。");
                SystemLogMessage?.Invoke($"[工單出站成功] 工單 {primaryWo} 帳務過帳完成。");

                // 出站成功後通知機台停止連續加工運轉
                if (OpcService.IsConnected)
                {
                    _ = Task.Run(async () =>
                    {
                        await OpcService.StopScheduleAsync().ConfigureAwait(false);
                    });
                }
            }
            else
            {
                _logger.Warn("TrackOut", $"工單出站失敗: 原因={reply.RtnMsg}");
                SystemLogMessage?.Invoke($"[工單出站失敗] 原因: {reply.RtnMsg}");
            }

            return reply;
        }

        public async Task<ReplyTrackOutReqPayload> TrackOutAsync(TrackOutReqPayload req, CancellationToken cancellationToken = default)
        {
            var msg = new TrackOutReqMessage
            {
                WorkOrder = req.WorkOrder ?? new List<string>(),
                CassetteID = req.CassetteID ?? new List<string>(),
                MaterialID = req.MaterialID,
                UserID = req.UserID,
                ToolingID = req.ToolingID,
                Qty = req.Qty,
                Result = req.Result,
                NGCode = req.NGCode
            };

            var replyMsg = await TrackOutAsync(msg, cancellationToken).ConfigureAwait(false);
            return new ReplyTrackOutReqPayload
            {
                TransactionID = replyMsg.TransactionID,
                Machine = replyMsg.Machine,
                RtnResult = replyMsg.RtnResult.ToString(),
                RtnMsg = replyMsg.RtnMsg,
                WorkOrder = replyMsg.WorkOrder?.FirstOrDefault() ?? string.Empty,
                CassetteID = replyMsg.CassetteID?.FirstOrDefault() ?? string.Empty,
                UserID = replyMsg.UserID,
                MaterialID = replyMsg.MaterialID,
                ToolingID = replyMsg.ToolingID
            };
        }

        public bool RemoveTrackedInOrder(string workOrder)
        {
            if (string.IsNullOrWhiteSpace(workOrder)) return false;

            int count = _trackedInOrders.RemoveAll(o => string.Equals(o.WorkOrder, workOrder, StringComparison.OrdinalIgnoreCase));
            if (count > 0)
            {
                if (string.Equals(CurrentOrder?.WorkOrder, workOrder, StringComparison.OrdinalIgnoreCase))
                {
                    CurrentOrder = _trackedInOrders.LastOrDefault();
                }
                _storageService.Save(_trackedInOrders);
                TrackInRemoved?.Invoke(workOrder);
                _logger.Warn("Coordinator", $"[手動解除進站] 人員手動強制自本機移除已進站工單: {workOrder}，剩餘已進站數: {_trackedInOrders.Count}");
                SystemLogMessage?.Invoke($"[手動移除進站] 已從本機清單強制移除工單: {workOrder}");
                return true;
            }
            return false;
        }

        public async Task<bool> SwitchOpcModeAsync(short mode, CancellationToken cancellationToken = default)
        {
            _logger.Info("SwitchMode", $"請求切換 OPC 模式為: {mode}");
            return await OpcService.ChangeOperatingModeAsync(mode, cancellationToken).ConfigureAwait(false);
        }

        #region OPC 事件監聽 ➔ EAP 上報

        private void OnOpcConnectionStateChanged(bool connected)
        {
            if (connected)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        // 稍候 500ms 確保節點初始化與工作階段完全就緒
                        await Task.Delay(500).ConfigureAwait(false);
                        if (OpcService.IsConnected && OpcService.CurrentOpcMode != MitsubishiOpcMode.OnlineSemiAuto)
                        {
                            SystemLogMessage?.Invoke("[OPC 自動切換模式] 偵測到機台 OPC 已連線，自動發送命令切換為 SEMI-AUTO 模式...");
                            bool ok = await SwitchOpcModeAsync((short)MitsubishiOpcMode.OnlineSemiAuto).ConfigureAwait(false);
                            if (ok)
                            {
                                _logger.Info("Coordinator", "機台 OPC 連線建立，已自動切換機台為 SEMI-AUTO 模式。");
                                SystemLogMessage?.Invoke("[OPC 自動切換模式成功] 機台已切換為 SEMI-AUTO 模式。");
                            }
                            else
                            {
                                _logger.Warn("Coordinator", "機台 OPC 連線建立，但切換 SEMI-AUTO 模式未成功。");
                                SystemLogMessage?.Invoke("[OPC 自動切換模式未成功] 機台切換 SEMI-AUTO 模式未成功 (機台可能非 Idle 狀態)。");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Warn("Coordinator", $"[OPC 自動切換模式例外] {ex.Message}");
                    }
                });
            }
        }

        private void OnOpcStatusLightChanged(MachineStatusLight oldLight, MachineStatusLight newLight)
        {
            _logger.Info("MachineStatus", $"OPC 燈號變更: {(int)oldLight} ({oldLight}) ➔ {(int)newLight} ({newLight})");
            SystemLogMessage?.Invoke($"[OPC 燈號變更] {(int)oldLight} ({oldLight}) ➔ {(int)newLight} ({newLight})");
            var report = new StatusChangeReportPayload
            {
                Machine = _config.Mqtt.EqID,
                OldStatus = ((int)oldLight).ToString(),
                NewStatus = ((int)newLight).ToString()
            };
            _ = EapService.PublishReportAsync(report);
        }

        private void OnOpcAlarmTriggered(string code, string msg, bool isStart)
        {
            if (string.IsNullOrWhiteSpace(msg) || msg == "警報解除")
            {
                msg = _config.Profile?.GetAlarmMessage(code, "未定義警報");
            }

            string stateStr = isStart ? "Start(發生)" : "End(解除)";
            if (isStart)
            {
                _logger.Warn("Alarm", $"代碼: {code}, 狀態: {stateStr}, 訊息: {msg}");
            }
            else
            {
                _logger.Info("Alarm", $"代碼: {code}, 狀態: {stateStr}, 訊息: {msg}");
            }
            SystemLogMessage?.Invoke($"[OPC 警報事件] 代碼: {code}, 狀態: {stateStr}, 訊息: {msg}");
            var report = new AlarmReportPayload
            {
                Machine = _config.Mqtt.EqID,
                AlarmStatus = isStart ? "Start" : "End",
                AlarmCode = code,
                AlarmMsg = msg,
                AlarmType = "A"
            };
            _ = EapService.PublishReportAsync(report);
        }

        private void OnOpcProcessedCountChanged(int oldVal, int newVal)
        {
            _logger.Info("ProcessData", $"OPC 加工計數遞增: {oldVal} ➔ {newVal} (預定: {OpcService.ScheduledCount})");
            SystemLogMessage?.Invoke($"[OPC 加工計數遞增] 完成片數: {oldVal} ➔ {newVal}");

            // 自動組裝製程資料上報
            var report = new ProcessDataReportPayload
            {
                Machine = _config.Mqtt.EqID,
                UserID = CurrentOperatorId,
                WorkOrder = CurrentOrder?.WorkOrder ?? string.Empty,
                RecipeID = CurrentOrder?.RecipeId ?? string.Empty,
                CassetteID = CurrentOrder?.CassetteId ?? string.Empty,
                Result = "PASS",
                ProcessTime = "0.0",
                ProcessData = new List<ProcessDataItem>
                {
                    new ProcessDataItem { Parameter = "ProcessedCount", Value = newVal.ToString() },
                    new ProcessDataItem { Parameter = "ScheduledCount", Value = OpcService.ScheduledCount.ToString() }
                }
            };
            _ = EapService.PublishReportAsync(report);
        }

        #endregion

        #region EAP 下行指令處理與配方交握

        private async Task<ReplyRemoteCMDMessage> HandleRemoteCommandAsync(RemoteCMDMessage cmd)
        {
            var reply = new ReplyRemoteCMDMessage
            {
                TransactionID = cmd.TransactionID,
                Machine = _config.Mqtt?.EqID ?? string.Empty,
                Date = DateTimeUtils.NowEapDate(),
                RemoteCMDType = cmd.RemoteCMDType
            };

            SystemLogMessage?.Invoke($"[EAP 遠端指令交握] 收到 RemoteCMD: Type={cmd.RemoteCMDType}, RecipeID={cmd.RecipeID}");
            _logger.Info("RemoteCMD", $"收到 EAP 遠端指令: Type={cmd.RemoteCMDType}, RecipeID={cmd.RecipeID}");

            try
            {
                switch (cmd.RemoteCMDType)
                {
                    case RemoteCommandType.PP_SELECT:
                        if (string.IsNullOrWhiteSpace(cmd.RecipeID))
                        {
                            reply.RtnResult = RtnResult.FAIL;
                            reply.RtnMsg = "RecipeID is empty";
                        }
                        else if (!OpcService.IsConnected)
                        {
                            reply.RtnResult = RtnResult.FAIL;
                            reply.RtnMsg = "OPC UA is disconnected, cannot deliver recipe to laser machine";
                        }
                        else
                        {
                            short sheetCount = CurrentOrder?.TotalQty > 0 ? (short)CurrentOrder.TotalQty : (short)5;
                            SystemLogMessage?.Invoke($"[配方切換交握] 正在下發 Recipe: {cmd.RecipeID} (片數: {sheetCount}) 至雷射機...");
                            bool success = await OpcService.DeliverRecipeAsync(cmd.RecipeID, sheetCount).ConfigureAwait(false);
                            if (success)
                            {
                                reply.RtnResult = RtnResult.PASS;
                                reply.RtnMsg = $"Recipe {cmd.RecipeID} delivered and acknowledged successfully";
                                SystemLogMessage?.Invoke($"[配方切換交握成功] 雷射機已確認切換至 Recipe: {cmd.RecipeID}");

                                if (CurrentOrder != null)
                                {
                                    CurrentOrder.RecipeId = cmd.RecipeID;
                                }
                            }
                            else
                            {
                                reply.RtnResult = RtnResult.FAIL;
                                reply.RtnMsg = $"DeliverRecipe failed or timed out for Recipe: {cmd.RecipeID}";
                                SystemLogMessage?.Invoke($"[配方切換交握失敗] 雷射機配方切換失敗或逾時！");
                            }
                        }
                        break;

                    case RemoteCommandType.START:
                        bool startOk = await OpcService.StartScheduleAsync().ConfigureAwait(false);
                        reply.RtnResult = startOk ? RtnResult.PASS : RtnResult.FAIL;
                        reply.RtnMsg = startOk ? "START command executed successfully" : "START command failed";
                        break;

                    case RemoteCommandType.STOP:
                        bool stopOk = await OpcService.StopScheduleAsync().ConfigureAwait(false);
                        reply.RtnResult = stopOk ? RtnResult.PASS : RtnResult.FAIL;
                        reply.RtnMsg = stopOk ? "STOP command executed successfully" : "STOP command failed";
                        break;

                    case RemoteCommandType.PAUSE:
                    case RemoteCommandType.RESUME:
                        reply.RtnResult = RtnResult.PASS;
                        reply.RtnMsg = $"{cmd.RemoteCMDType} acknowledged";
                        break;

                    default:
                        reply.RtnResult = RtnResult.FAIL;
                        reply.RtnMsg = $"Unsupported command type: {cmd.RemoteCMDType}";
                        break;
                }
            }
            catch (Exception ex)
            {
                reply.RtnResult = RtnResult.FAIL;
                reply.RtnMsg = $"Exception executing {cmd.RemoteCMDType}: {ex.Message}";
                _logger.Error("RemoteCMD", $"執行遠端指令異常: {ex.Message} {ex.StackTrace}", _config.Mqtt?.EqID);
            }

            return reply;
        }

        private void OnRemoteCommandReceived(RemoteCMDMessage cmd)
        {
            SystemLogMessage?.Invoke($"[EAP 遠端指令通知] Type: {cmd.RemoteCMDType}, Recipe: {cmd.RecipeID}");
        }

        private void OnTerminalDisplayReceived(TerminalDisplayMessage msg)
        {
            SystemLogMessage?.Invoke($"[EAP 終端訊息] {msg.Message}");
            TerminalMessageNotified?.Invoke(msg.Message);
        }

        private void OnTimeCalibrationReceived(string serverDate)
        {
            SystemLogMessage?.Invoke($"[EAP 時間校正] 收到標準時間: {serverDate}");
        }

        #endregion

        public void Dispose()
        {
            EapService?.Dispose();
            OpcService?.Dispose();
        }
    }
}
