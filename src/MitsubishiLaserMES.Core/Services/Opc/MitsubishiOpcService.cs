using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MitsubishiLaserMES.Core.Common;
using MitsubishiLaserMES.Core.Models.Config;
using MitsubishiLaserOpc.Clients;
using MitsubishiLaserOpc.Interfaces;
using MitsubishiLaserOpc.Models;
using MitsubishiLaserOpc.Nodes;
using MitsubishiLaserOpc.Enums;
using MitsubishiLaserOpc.Services;

namespace MitsubishiLaserMES.Core.Services.Opc
{
    public class MitsubishiOpcService : IOpcService
    {
        private readonly OpcSettings _settings;
        private IOpcUaClient _client;
        private NodeDiagnosticService _diagnosticService;
        private RecipeHandshakeService _handshakeService;
        private OpcWatchdogService _watchdog;

        private CancellationTokenSource _pollCts;
        private Task _pollTask;
        private readonly Dictionary<string, string> _activeAlarms = new Dictionary<string, string>();
        private bool _lastGetRecipeReq = false;

        public bool IsConnected { get; private set; }
        public bool IsVirtual => _settings.UseVirtualSimulator;
        public MitsubishiMachineStatusCode CurrentStatusCode { get; private set; } = MitsubishiMachineStatusCode.Idle;
        public MachineStatusLight CurrentStatusLight { get; private set; } = MachineStatusLight.Standby;
        public int ProcessedCount { get; private set; } = 0;
        public int ScheduledCount { get; private set; } = 0;
        public string ActiveLotId { get; private set; } = string.Empty;
        public string ActiveProgramFile { get; private set; } = string.Empty;
        public MitsubishiOpcMode CurrentOpcMode { get; private set; } = MitsubishiOpcMode.Offline;

        public event Action<bool> ConnectionStateChanged;
        public event Action<MitsubishiOpcMode> OpcModeChanged;
        public event Action<MachineStatusLight, MachineStatusLight> StatusLightChanged;
        public event Action<int, int> ProcessedCountChanged;
        public event Action<string, string, bool> AlarmTriggered;
        public event Action<string> RecipeRequestedByMachine;
        public event Action<string> LogMessage;

        public MitsubishiOpcService(OpcSettings settings)
        {
            _settings = settings ?? new OpcSettings();
        }

        public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
        {
            if (IsConnected)
            {
                LogMessage?.Invoke("[OPC] 目前已處於連線狀態，無需重複連線。");
                return true;
            }

            try
            {
                LogMessage?.Invoke($"[OPC] 正在建立連線... (模式: {(IsVirtual ? "虛擬模擬器" : "實機 OPC-UA")})");
                if (IsVirtual)
                {
                    _client = new VirtualOpcUaClient();
                }
                else
                {
                    var options = new MitsubishiOpcUaOptions
                    {
                        EndpointUrl = _settings.EndpointUrl,
                        UseSecurity = _settings.UseSecurity,
                        MinimumCertificateKeySize = _settings.MinimumCertificateKeySize,
                        RejectSha1SignedCertificates = _settings.RejectSha1SignedCertificates,
                        AutoAcceptUntrustedCertificates = _settings.AutoAcceptUntrustedCertificates
                    };
                    var realClient = new MitsubishiOpcUaClient(options);
                    await realClient.ConnectAsync(cancellationToken).ConfigureAwait(false);
                    _client = realClient;
                }

                _diagnosticService = new NodeDiagnosticService(_client);
                _handshakeService = new RecipeHandshakeService(_diagnosticService);

                // 啟動原廠 WatchDog 心跳
                _watchdog = new OpcWatchdogService(_client);
                _watchdog.Start(_settings.WatchdogIntervalSec);

                IsConnected = true;
                ConnectionStateChanged?.Invoke(true);
                LogMessage?.Invoke("[OPC] 連線成功，WatchDog 心跳已啟動。");

                // 啟動定時輪詢監控
                _pollCts = new CancellationTokenSource();
                _pollTask = Task.Run(() => PollingLoopAsync(_pollCts.Token));

                return true;
            }
            catch (Exception ex)
            {
                IsConnected = false;
                ConnectionStateChanged?.Invoke(false);
                LogMessage?.Invoke($"[OPC 連線失敗] {ex.Message}");
                return false;
            }
        }

        public Task DisconnectAsync()
        {
            try
            {
                _pollCts?.Cancel();
                _watchdog?.Stop();
                if (_client is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            catch { }
            finally
            {
                IsConnected = false;
                ConnectionStateChanged?.Invoke(false);
                LogMessage?.Invoke("[OPC] 已斷開連線。");
            }
            return Task.CompletedTask;
        }

        public Task<bool> DeliverRecipeAsync(string recipeId, short sheetCount, CancellationToken cancellationToken = default)
        {
            return HandshakeRecipeToMachineAsync(recipeId, "*****", sheetCount, cancellationToken);
        }

        public Task<bool> DeliverRecipeAsync(string programFile, string conditionFile, short sheetCount, CancellationToken cancellationToken = default)
        {
            return HandshakeRecipeToMachineAsync(programFile, conditionFile, sheetCount, cancellationToken);
        }

        public async Task<bool> HandshakeRecipeToMachineAsync(string programFile, string conditionFile, short sheetCount, CancellationToken cancellationToken = default)
        {
            if (!IsConnected || _client == null || _diagnosticService == null)
            {
                LogMessage?.Invoke("[OPC 交握失敗] 未連線，無法執行配方交握。");
                return false;
            }

            try
            {
                // 原廠規格化：ProgramFile 與 ConditionFile 僅傳純檔名或相對路徑，避免絕對路徑引發機台排程載入異常
                string prog = NormalizeRecipeFileName(programFile);
                string cond = string.IsNullOrWhiteSpace(conditionFile) ? "*****" : NormalizeRecipeFileName(conditionFile);
                short sheets = sheetCount <= 0 ? (short)5 : sheetCount;

                LogMessage?.Invoke($"[OPC 配方交握開始] 依原廠時序寫入配方參數: ProgramFile='{prog}', ConditionFile='{cond}', SheetNum={sheets}");

                // 1. 寫入 ProgramFile
                var pRes = await _diagnosticService.WriteAsync(LaserOpcNode.RecipeProgramFile, prog, cancellationToken).ConfigureAwait(false);
                if (!pRes.Succeeded)
                {
                    LogMessage?.Invoke($"[OPC 配方交握失敗] 寫入 ProgramFile 失敗: {pRes.Error}");
                    return false;
                }

                // 2. 寫入 ConditionFile
                var cRes = await _diagnosticService.WriteAsync(LaserOpcNode.RecipeConditionFile, cond, cancellationToken).ConfigureAwait(false);
                if (!cRes.Succeeded)
                {
                    LogMessage?.Invoke($"[OPC 配方交握失敗] 寫入 ConditionFile 失敗: {cRes.Error}");
                    return false;
                }

                // 3. 寫入 SheetNum
                var sRes = await _diagnosticService.WriteAsync(LaserOpcNode.RecipeSheetNum, sheets, cancellationToken).ConfigureAwait(false);
                if (!sRes.Succeeded)
                {
                    LogMessage?.Invoke($"[OPC 配方交握失敗] 寫入 SheetNum 失敗: {sRes.Error}");
                    return false;
                }

                // 4. 置位 GetRecipe.Ack = 1
                var ackRes = await _diagnosticService.WriteAsync(LaserOpcNode.GetRecipeAck, (short)1, cancellationToken).ConfigureAwait(false);
                if (!ackRes.Succeeded)
                {
                    LogMessage?.Invoke($"[OPC 配方交握失敗] 寫入 GetRecipe.Ack=1 失敗: {ackRes.Error}");
                    return false;
                }

                LogMessage?.Invoke("[OPC 配方交握] GetRecipe.Ack = 1 已送出，等待機台復位 GetRecipe.Req (逾時 10 秒)...");

                // 5. 等待機台將 GetRecipe.Req 降為 false
                bool machineAcked = false;
                DateTime timeoutTime = DateTime.UtcNow.AddSeconds(10);
                while (DateTime.UtcNow < timeoutTime && !cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(100, cancellationToken).ConfigureAwait(false);
                    var checkReq = await _client.ReadAsync(LaserOpcNodeCatalog.Get(LaserOpcNode.GetRecipeRequest), cancellationToken).ConfigureAwait(false);
                    if (checkReq.Succeeded && checkReq.Value != null)
                    {
                        if (!Convert.ToBoolean(checkReq.Value))
                        {
                            machineAcked = true;
                            _lastGetRecipeReq = false;
                            break;
                        }
                    }
                }

                // 6. 原廠規範重要步驟：Host 必須將 GetRecipe.Ack 清零復位 (0)
                try
                {
                    await _diagnosticService.WriteAsync(LaserOpcNode.GetRecipeAck, (short)0, cancellationToken).ConfigureAwait(false);
                    LogMessage?.Invoke("[OPC 配方交握] Host 已成功復位 GetRecipe.Ack = 0。");
                }
                catch (Exception exReset)
                {
                    LogMessage?.Invoke($"[OPC 配方交握警告] 復位 GetRecipe.Ack=0 發生異常: {exReset.Message}");
                }

                if (machineAcked)
                {
                    LogMessage?.Invoke($"[OPC 配方交握成功] 機台已確認接收配方，Req 已降為 false，交握流程圓滿完成！");
                    return true;
                }
                else
                {
                    LogMessage?.Invoke($"[OPC 配方交握逾時] 等待機台 GetRecipe.Req 復歸逾時 (10s)，交握可能未被機台完全載入。");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"[OPC 配方交握例外] {ex.Message}");
                try
                {
                    await _diagnosticService.WriteAsync(LaserOpcNode.GetRecipeAck, (short)0, CancellationToken.None).ConfigureAwait(false);
                }
                catch { }
                return false;
            }
        }

        private static string NormalizeRecipeFileName(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return string.Empty;
            string trimmed = filePath.Trim();
            if (trimmed.Contains("\\") || trimmed.Contains("/"))
            {
                return System.IO.Path.GetFileName(trimmed);
            }
            return trimmed;
        }

        public async Task<bool> StartScheduleAsync(CancellationToken cancellationToken = default)
        {
            if (!IsConnected || _handshakeService == null) return false;
            try
            {
                LogMessage?.Invoke("[OPC] 發送連續運轉要求 (StartScheduleRequest)...");
                var result = await _handshakeService.StartAutoScheduleAsync(cancellationToken).ConfigureAwait(false);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"[OPC] 啟動運轉例外: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> StopScheduleAsync(CancellationToken cancellationToken = default)
        {
            // 透過切換模式至 Idle 或寫入 Ack 清除
            return await Task.FromResult(true);
        }

        public async Task<bool> ChangeOperatingModeAsync(short mode, CancellationToken cancellationToken = default)
        {
            if (!IsConnected || _diagnosticService == null) return false;
            try
            {
                await _diagnosticService.WriteAsync(LaserOpcNode.RequestedOpcMode, mode, cancellationToken).ConfigureAwait(false);
                var ack = await _diagnosticService.WriteAsync(LaserOpcNode.ChangeOpcModeRequest, true, cancellationToken).ConfigureAwait(false);
                if (ack.Succeeded)
                {
                    await Task.Delay(100, cancellationToken).ConfigureAwait(false);
                    var modeRes = await _client.ReadAsync(LaserOpcNodeCatalog.Get(LaserOpcNode.MachineOpcMode), cancellationToken).ConfigureAwait(false);
                    if (modeRes.Succeeded && modeRes.Value != null)
                    {
                        short modeVal = Convert.ToInt16(modeRes.Value);
                        if (Enum.IsDefined(typeof(MitsubishiOpcMode), (int)modeVal))
                        {
                            var newMode = (MitsubishiOpcMode)modeVal;
                            if (newMode != CurrentOpcMode)
                            {
                                CurrentOpcMode = newMode;
                                OpcModeChanged?.Invoke(newMode);
                            }
                        }
                    }
                    // 復歸 ChangeOpcModeRequest
                    await _diagnosticService.WriteAsync(LaserOpcNode.ChangeOpcModeRequest, false, cancellationToken).ConfigureAwait(false);
                }
                return ack.Succeeded;
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"[OPC] 切換模式例外: {ex.Message}");
                return false;
            }
        }

        private async Task PollingLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await PollMachineStateAsync(token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    LogMessage?.Invoke($"[OPC 輪詢警告] {ex.Message}");
                }

                try
                {
                    await Task.Delay(_settings.PollingIntervalMs, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private async Task PollMachineStateAsync(CancellationToken token)
        {
            if (_client == null || !IsConnected) return;

            // 0. 讀取機台 OPC 模式
            var modeRes = await _client.ReadAsync(LaserOpcNodeCatalog.Get(LaserOpcNode.MachineOpcMode), token).ConfigureAwait(false);
            if (modeRes.Succeeded && modeRes.Value != null)
            {
                short modeVal = Convert.ToInt16(modeRes.Value);
                if (Enum.IsDefined(typeof(MitsubishiOpcMode), (int)modeVal))
                {
                    var newMode = (MitsubishiOpcMode)modeVal;
                    if (newMode != CurrentOpcMode)
                    {
                        CurrentOpcMode = newMode;
                        OpcModeChanged?.Invoke(newMode);
                    }
                }
            }

            // 1. 讀取狀態碼
            var statusRes = await _client.ReadAsync(LaserOpcNodeCatalog.Get(LaserOpcNode.MachineStatusCode), token).ConfigureAwait(false);
            if (statusRes.Succeeded && statusRes.Value != null)
            {
                int codeVal = Convert.ToInt32(statusRes.Value);
                CurrentStatusCode = (MitsubishiMachineStatusCode)codeVal;
            }

            // 2. 讀取三色燈
            var greenRes = await _client.ReadAsync(LaserOpcNodeCatalog.Get(LaserOpcNode.WarningLampGreen), token).ConfigureAwait(false);
            var yellowRes = await _client.ReadAsync(LaserOpcNodeCatalog.Get(LaserOpcNode.WarningLampYellow), token).ConfigureAwait(false);
            var redRes = await _client.ReadAsync(LaserOpcNodeCatalog.Get(LaserOpcNode.WarningLampRed), token).ConfigureAwait(false);

            bool isGreen = greenRes.Succeeded && Convert.ToBoolean(greenRes.Value);
            bool isYellow = yellowRes.Succeeded && Convert.ToBoolean(yellowRes.Value);
            bool isRed = redRes.Succeeded && Convert.ToBoolean(redRes.Value);

            // 映射至 13 種狀態燈號
            MachineStatusLight newLight = DetermineStatusLight(CurrentStatusCode, isGreen, isYellow, isRed);
            if (newLight != CurrentStatusLight)
            {
                var oldLight = CurrentStatusLight;
                CurrentStatusLight = newLight;
                StatusLightChanged?.Invoke(oldLight, newLight);
            }

            // 3. 讀取加工片數
            var procRes = await _client.ReadAsync(LaserOpcNodeCatalog.Get(LaserOpcNode.ProcessedCount), token).ConfigureAwait(false);
            if (procRes.Succeeded && procRes.Value != null)
            {
                int newProc = Convert.ToInt32(procRes.Value);
                if (newProc != ProcessedCount)
                {
                    int oldProc = ProcessedCount;
                    ProcessedCount = newProc;
                    ProcessedCountChanged?.Invoke(oldProc, newProc);
                }
            }

            var schedRes = await _client.ReadAsync(LaserOpcNodeCatalog.Get(LaserOpcNode.ScheduledCount), token).ConfigureAwait(false);
            if (schedRes.Succeeded && schedRes.Value != null)
            {
                ScheduledCount = Convert.ToInt32(schedRes.Value);
            }

            var lotRes = await _client.ReadAsync(LaserOpcNodeCatalog.Get(LaserOpcNode.ActiveLotId), token).ConfigureAwait(false);
            if (lotRes.Succeeded && lotRes.Value != null)
            {
                ActiveLotId = lotRes.Value.ToString();
            }

            var prgRes = await _client.ReadAsync(LaserOpcNodeCatalog.Get(LaserOpcNode.ActiveProgramFile), token).ConfigureAwait(false);
            if (prgRes.Succeeded && prgRes.Value != null)
            {
                ActiveProgramFile = prgRes.Value.ToString();
            }

            // 3.5. 檢查機台端 GetRecipe.Req (上升緣 0->1 觸發配方交握)
            var reqRes = await _client.ReadAsync(LaserOpcNodeCatalog.Get(LaserOpcNode.GetRecipeRequest), token).ConfigureAwait(false);
            if (reqRes.Succeeded && reqRes.Value != null)
            {
                bool currentReq = Convert.ToBoolean(reqRes.Value);
                if (currentReq && !_lastGetRecipeReq)
                {
                    string reqLot = string.Empty;
                    var lotReqRes = await _client.ReadAsync(LaserOpcNodeCatalog.Get(LaserOpcNode.RemoteLotId), token).ConfigureAwait(false);
                    if (lotReqRes.Succeeded && lotReqRes.Value != null)
                    {
                        reqLot = lotReqRes.Value.ToString();
                    }
                    LogMessage?.Invoke($"[OPC 機台請求配方] 偵測到機台發出 GetRecipe.Req = true (LotID: '{reqLot}')，觸發配方交握程序。");
                    RecipeRequestedByMachine?.Invoke(reqLot);
                }
                _lastGetRecipeReq = currentReq;
            }

            // 4. 檢查警報 Slot 000 ~ 009
            await CheckAlarmsAsync(token).ConfigureAwait(false);
        }

        private MachineStatusLight DetermineStatusLight(MitsubishiMachineStatusCode status, bool isGreen, bool isYellow, bool isRed)
        {
            if (isRed || status == MitsubishiMachineStatusCode.Stop)
            {
                return MachineStatusLight.Alarm;
            }
            if (status == MitsubishiMachineStatusCode.Running)
            {
                return MachineStatusLight.AutoRunning;
            }
            if (status == MitsubishiMachineStatusCode.Maintain)
            {
                return MachineStatusLight.Tuning;
            }
            if (status == MitsubishiMachineStatusCode.Ready)
            {
                return MachineStatusLight.Standby;
            }
            if (status == MitsubishiMachineStatusCode.MachineDown)
            {
                return MachineStatusLight.SafetyStop;
            }
            return MachineStatusLight.Standby;
        }

        private async Task CheckAlarmsAsync(CancellationToken token)
        {
            var currentPollAlarms = new HashSet<string>();

            for (int i = 0; i < 10; i++)
            {
                var noDesc = new NodeDescriptor(
                    LaserOpcNode.MachineType,
                    LaserOpcNodeCatalog.FormatGroupTag(LaserOpcNodeGroup.Alarm, i, "No"),
                    OpcValueType.Int32,
                    NodeDirection.MachineToHost,
                    $"Alarm {i:000} No");

                var readNo = await _client.ReadAsync(noDesc, token).ConfigureAwait(false);
                if (readNo.Succeeded && readNo.Value != null)
                {
                    long no = Convert.ToInt64(readNo.Value);
                    if (no > 0)
                    {
                        string codeStr = no.ToString("D4");
                        currentPollAlarms.Add(codeStr);

                        if (!_activeAlarms.ContainsKey(codeStr))
                        {
                            // 讀取警報訊息
                            var msgDesc = new NodeDescriptor(
                                LaserOpcNode.MachineType,
                                LaserOpcNodeCatalog.FormatGroupTag(LaserOpcNodeGroup.Alarm, i, "Message"),
                                OpcValueType.String,
                                NodeDirection.MachineToHost,
                                $"Alarm {i:000} Message");
                            var readMsg = await _client.ReadAsync(msgDesc, token).ConfigureAwait(false);
                            string msgStr = readMsg.Value?.ToString() ?? "機台異常";

                            _activeAlarms[codeStr] = msgStr;
                            AlarmTriggered?.Invoke(codeStr, msgStr, true);
                        }
                    }
                }
            }

            // 檢查復歸 (原本在 _activeAlarms 但不在 currentPollAlarms)
            var resolved = _activeAlarms.Keys.Where(a => !currentPollAlarms.Contains(a)).ToList();
            foreach (var code in resolved)
            {
                string originalMsg = _activeAlarms.TryGetValue(code, out var m) ? m : string.Empty;
                _activeAlarms.Remove(code);
                AlarmTriggered?.Invoke(code, originalMsg, false);
            }
        }

        public void Dispose()
        {
            DisconnectAsync().GetAwaiter().GetResult();
            _pollCts?.Dispose();
        }
    }
}
