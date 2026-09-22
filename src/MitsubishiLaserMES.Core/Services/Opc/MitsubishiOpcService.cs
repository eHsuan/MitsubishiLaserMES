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
        private readonly HashSet<string> _activeAlarms = new HashSet<string>();

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
                        UseSecurity = _settings.UseSecurity
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

        public async Task<bool> DeliverRecipeAsync(string recipeId, short sheetCount, CancellationToken cancellationToken = default)
        {
            if (!IsConnected || _handshakeService == null)
            {
                LogMessage?.Invoke("[OPC] 未連線，無法下發配方。");
                return false;
            }

            try
            {
                // 依原廠規範 P.11，未用條件檔時 ConditionFile 固定帶 "*****"
                var recipe = new RecipeDefinition(
                    recipeId: recipeId,
                    version: "1.0",
                    programFile: recipeId,
                    conditionFile: "*****",
                    sheetCount: sheetCount
                );

                LogMessage?.Invoke($"[OPC] 開始執行 Recipe 交握: ProgramFile={recipeId}, ConditionFile=*****, SheetNum={sheetCount}");
                var result = await _handshakeService.DeliverRecipeAsync(recipe, cancellationToken).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    LogMessage?.Invoke("[OPC] Recipe 交握成功 (Ack=1 已送出)。");
                    return true;
                }
                else
                {
                    LogMessage?.Invoke($"[OPC] Recipe 交握失敗: {result.Error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"[OPC] 下發配方例外: {ex.Message}");
                return false;
            }
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

                        if (!_activeAlarms.Contains(codeStr))
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

                            _activeAlarms.Add(codeStr);
                            AlarmTriggered?.Invoke(codeStr, msgStr, true);
                        }
                    }
                }
            }

            // 檢查復歸 (原本在 _activeAlarms 但不在 currentPollAlarms)
            var resolved = _activeAlarms.Where(a => !currentPollAlarms.Contains(a)).ToList();
            foreach (var code in resolved)
            {
                _activeAlarms.Remove(code);
                AlarmTriggered?.Invoke(code, "警報解除", false);
            }
        }

        public void Dispose()
        {
            DisconnectAsync().GetAwaiter().GetResult();
            _pollCts?.Dispose();
        }
    }
}
