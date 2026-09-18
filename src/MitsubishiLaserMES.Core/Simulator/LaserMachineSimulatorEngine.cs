using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MitsubishiLaserOpc.Enums;
using MitsubishiLaserOpc.Nodes;

namespace MitsubishiLaserMES.Core.Simulator
{
    /// <summary>
    /// 三菱 CO2 雷射鑽孔機核心模擬引擎 (完全依據 S26888-C 原廠規格書)
    /// </summary>
    public class LaserMachineSimulatorEngine : ILaserMachineSimulator
    {
        public static LaserMachineSimulatorEngine SharedInstance { get; } = new LaserMachineSimulatorEngine();

        private readonly object _lock = new object();
        private readonly SimulatorAlarmItem[] _alarms = new SimulatorAlarmItem[10];
        private readonly Random _rand = new Random();

        // 計時器
        private readonly Timer _watchDogTimer;
        private readonly Timer _processingTimer;

        // 模式與狀態 (0: MachineDown/Alarm, 1: Ready, 2: Running, 3: Idle, 4: Maintain, 5: Stop)
        public MachineOperatingMode OpcMode { get; private set; } = MachineOperatingMode.OnlineAuto;
        public MachineStatus StatusCode { get; private set; } = MachineStatus.Idle;
        public bool WarningLampGreen { get; private set; } = false;
        public bool WarningLampYellow { get; private set; } = false;
        public bool WarningLampRed { get; private set; } = false;

        // WatchDog
        public ushort LastHostWatchDog { get; private set; } = 0;
        public int WatchDogCountdownSec { get; private set; } = 10;
        public bool IsWatchDogTimeout { get; private set; } = false;
        public bool IsWatchDogEnabled { get; set; } = true;
        public bool HasReceivedFirstHeartbeat { get; private set; } = false;

        // 配方請求與交握
        public string RemoteLotId { get; private set; } = string.Empty;
        public bool GetRecipeRequest { get; private set; } = false;
        public string RequestedProgramFile { get; private set; } = string.Empty;
        public string RequestedConditionFile { get; private set; } = string.Empty;
        public short RequestedSheetNum { get; private set; } = -1;
        public short GetRecipeAck { get; private set; } = 0;
        public short ChangeOpcModeAck { get; private set; } = 0;

        // 運轉進行中資料
        public string ActiveLotId { get; private set; } = string.Empty;
        public string ActiveProgramFile { get; private set; } = string.Empty;
        public string ActiveConditionFile { get; private set; } = string.Empty;
        public short ScheduledCount { get; private set; } = 0;
        public short ProcessedCount { get; private set; } = 0;
        public short UnProcessedCount { get; private set; } = 0;
        public double CurrentCycleTime { get; private set; } = 0.0;

        // 感測與雷射參數
        public int ActivePower { get; private set; } = 5500;
        public int ActiveFrequency { get; private set; } = 100;
        public float ActivePulseWidth { get; private set; } = 49.0f;
        public float TableAdsorbPressure { get; private set; } = -10.5f;
        public float LensTemp { get; private set; } = 24.2f;

        public IReadOnlyList<SimulatorAlarmItem> Alarms => _alarms;

        public event Action StateChanged;
        public event Action<string> LogEmitted;

        public LaserMachineSimulatorEngine()
        {
            // 初始化 10 組警報 Slot
            for (int i = 0; i < 10; i++)
            {
                _alarms[i] = new SimulatorAlarmItem
                {
                    SlotIndex = i,
                    AlarmNo = 0,
                    AlarmMessage = string.Empty,
                    IsActive = false
                };
            }

            // 啟動 WatchDog 1 秒倒數計時器
            _watchDogTimer = new Timer(WatchDogTick, null, 1000, 1000);

            // 啟動加工循環計時器 (每 2.5 秒一次)
            _processingTimer = new Timer(ProcessingTick, null, 2500, 2500);

            UpdateLamps();
        }

        #region ILaserMachineSimulator 實作

        public void OperatorInputLot(string lotId)
        {
            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(lotId)) return;
                RemoteLotId = lotId;
                GetRecipeRequest = true;
                GetRecipeAck = 0;
                Log($"[操作員刷卡] 輸入批號: {lotId}，發出 GetRecipe.Req = true，等待上位機核發配方。");
                NotifyStateChanged();
            }
        }

        public bool TriggerAlarm(long alarmNo, string message, string type = "A")
        {
            lock (_lock)
            {
                var slot = _alarms.FirstOrDefault(a => !a.IsActive);
                if (slot == null)
                {
                    Log($"[警報觸發失敗] 10 組警報 Slot 已滿，無法注入警報 #{alarmNo}");
                    return false;
                }

                slot.AlarmNo = alarmNo;
                slot.AlarmMessage = message;
                slot.AlarmType = type;
                slot.TriggerTime = DateTime.Now;
                slot.IsActive = true;

                SetMachineStatus(MachineStatus.MachineDown);
                Log($"[警報觸發] Slot[{slot.SlotIndex}] 注入警報: #{alarmNo} - {message} (Type: {type})");
                NotifyStateChanged();
                return true;
            }
        }

        public void ResetAlarm(int slotIndex = -1)
        {
            lock (_lock)
            {
                WatchDogCountdownSec = 10;
                IsWatchDogTimeout = false;

                if (slotIndex >= 0 && slotIndex < 10)
                {
                    _alarms[slotIndex].IsActive = false;
                    _alarms[slotIndex].AlarmNo = 0;
                    _alarms[slotIndex].AlarmMessage = string.Empty;
                    Log($"[警報解除] Slot[{slotIndex}] 警報已清除。");
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        _alarms[i].IsActive = false;
                        _alarms[i].AlarmNo = 0;
                        _alarms[i].AlarmMessage = string.Empty;
                    }
                    Log("[警報復歸] 執行 AL-RESET，所有警報 Slot 已全部清除。");
                }

                if (!_alarms.Any(a => a.IsActive))
                {
                    if (StatusCode == MachineStatus.MachineDown)
                    {
                        SetMachineStatus(MachineStatus.Idle);
                    }
                }
                NotifyStateChanged();
            }
        }

        public void FeedWatchDog(ushort? customDog = null)
        {
            lock (_lock)
            {
                ushort dog = customDog ?? (ushort)(LastHostWatchDog + 1);
                LastHostWatchDog = dog;
                WatchDogCountdownSec = 10;
                HasReceivedFirstHeartbeat = true;

                if (IsWatchDogTimeout)
                {
                    IsWatchDogTimeout = false;
                    Log($"[WatchDog 心跳恢復] 上位機心跳更新 (Dog: {dog})，解除通訊異常。");
                    ResetAlarmByNo(9999);
                }
                NotifyStateChanged();
            }
        }

        private void ResetAlarmByNo(long alarmNo)
        {
            for (int i = 0; i < 10; i++)
            {
                if (_alarms[i].IsActive && _alarms[i].AlarmNo == alarmNo)
                {
                    _alarms[i].IsActive = false;
                    _alarms[i].AlarmNo = 0;
                    _alarms[i].AlarmMessage = string.Empty;
                    Log($"[自動清除警報] 已自動清除 #{alarmNo} 警報。");
                }
            }
            if (!_alarms.Any(a => a.IsActive) && StatusCode == MachineStatus.MachineDown)
            {
                SetMachineStatus(MachineStatus.Idle);
            }
        }

        public bool StartSchedule()
        {
            lock (_lock)
            {
                if (StatusCode == MachineStatus.MachineDown)
                {
                    Log("[啟動失敗] 機台目前處於警報停機狀態，無法啟動加工。");
                    return false;
                }

                if (string.IsNullOrEmpty(ActiveProgramFile))
                {
                    Log("[啟動失敗] 尚未載入加工程式，無法啟動加工。");
                    return false;
                }

                SetMachineStatus(MachineStatus.Running);
                Log($"[機台啟動] 開始連續運轉加工，批號: {ActiveLotId}，加工程式: {ActiveProgramFile}，排程片數: {ScheduledCount}");
                NotifyStateChanged();
                return true;
            }
        }

        public void StopSchedule()
        {
            lock (_lock)
            {
                SetMachineStatus(MachineStatus.Idle);
                Log("[機台停止] 操作員或上位機停止連續運轉加工。");
                NotifyStateChanged();
            }
        }

        public void SetMachineStatus(MachineStatus status)
        {
            lock (_lock)
            {
                StatusCode = status;
                UpdateLamps();
                Log($"[機台狀態切換] 狀態變更為: {status}");
                NotifyStateChanged();
            }
        }

        public void SetOperatingMode(MachineOperatingMode mode)
        {
            lock (_lock)
            {
                OpcMode = mode;
                if (mode == MachineOperatingMode.Offline)
                {
                    WatchDogCountdownSec = 10;
                    if (IsWatchDogTimeout)
                    {
                        IsWatchDogTimeout = false;
                        ResetAlarmByNo(9999);
                    }
                }
                Log($"[模式切換] 機台 OPC 模式切換為: {mode}");
                NotifyStateChanged();
            }
        }

        public void SimulateTargetFailure()
        {
            lock (_lock)
            {
                if (StatusCode == MachineStatus.Running)
                {
                    UnProcessedCount++;
                    Log($"[靶位失敗] 視覺對位抓靶失敗，未加工數 (UnProcessedCount) 累加至: {UnProcessedCount}");
                    NotifyStateChanged();
                }
            }
        }

        #endregion

        #region OPC 讀寫交握核心處理

        public object HandleHostRead(NodeDescriptor node)
        {
            if (node == null) return null;

            lock (_lock)
            {
                // 1. 群組警報標籤解析 MACHINE.Status.Alarm.{index:000}.{field}
                if (!string.IsNullOrEmpty(node.SpecTag) && node.SpecTag.StartsWith("MACHINE.Status.Alarm."))
                {
                    string[] parts = node.SpecTag.Split('.');
                    if (parts.Length >= 5 && int.TryParse(parts[3], out int slotIdx) && slotIdx >= 0 && slotIdx < 10)
                    {
                        var alarm = _alarms[slotIdx];
                        string field = parts[4];
                        if (field.Equals("No", StringComparison.OrdinalIgnoreCase)) return alarm.AlarmNo;
                        if (field.Equals("Message", StringComparison.OrdinalIgnoreCase)) return alarm.AlarmMessage;
                        if (field.Equals("Date", StringComparison.OrdinalIgnoreCase)) return alarm.TriggerTime.ToString("yyyy/MM/dd");
                        if (field.Equals("Time", StringComparison.OrdinalIgnoreCase)) return alarm.TriggerTime.ToString("HH:mm:ss");
                        if (field.Equals("Type", StringComparison.OrdinalIgnoreCase)) return alarm.AlarmType;
                    }
                }

                // 2. 標準枚舉節點
                switch (node.Node)
                {
                    case LaserOpcNode.MachineOpcMode:
                        return (short)OpcMode;

                    case LaserOpcNode.MachineStatusCode:
                        return (int)StatusCode;

                    case LaserOpcNode.WarningLampGreen:
                        return WarningLampGreen;

                    case LaserOpcNode.WarningLampYellow:
                        return WarningLampYellow;

                    case LaserOpcNode.WarningLampRed:
                        return WarningLampRed;

                    case LaserOpcNode.RemoteLotId:
                        return RemoteLotId;

                    case LaserOpcNode.GetRecipeRequest:
                        return GetRecipeRequest;

                    case LaserOpcNode.GetRecipeAck:
                        return GetRecipeAck;

                    case LaserOpcNode.ChangeOpcModeAck:
                        return ChangeOpcModeAck;

                    case LaserOpcNode.ActiveLotId:
                        return ActiveLotId;

                    case LaserOpcNode.ActiveProgramFile:
                        return ActiveProgramFile;

                    case LaserOpcNode.ActiveConditionFile:
                        return ActiveConditionFile;

                    case LaserOpcNode.ScheduledCount:
                        return ScheduledCount;

                    case LaserOpcNode.ProcessedCount:
                        return ProcessedCount;

                    case LaserOpcNode.UnProcessedCount:
                        return UnProcessedCount;

                    case LaserOpcNode.ActivePower:
                        return ActivePower;

                    case LaserOpcNode.ActiveFrequency:
                        return ActiveFrequency;

                    case LaserOpcNode.ActivePulseWidth:
                        return ActivePulseWidth;

                    case LaserOpcNode.TableAdsorbPressure:
                        return TableAdsorbPressure;

                    case LaserOpcNode.FthetaLensStageRTemp:
                    case LaserOpcNode.FthetaLensStageLTemp:
                        return LensTemp;

                    case LaserOpcNode.MachineType:
                        return "ML605GTF";

                    case LaserOpcNode.MachineNumber:
                        return "S26888-C";

                    case LaserOpcNode.OperatorId:
                        return "OP001";

                    default:
                        return null;
                }
            }
        }

        public bool HandleHostWrite(NodeDescriptor node, object value)
        {
            if (node == null) return false;

            lock (_lock)
            {
                switch (node.Node)
                {
                    // 1. Host WatchDog 寫入
                    case LaserOpcNode.HostWatchDog:
                        ushort dog = Convert.ToUInt16(value);
                        FeedWatchDog(dog);
                        return true;

                    // 2. 模式切換請求
                    case LaserOpcNode.RequestedOpcMode:
                        return true;

                    case LaserOpcNode.ChangeOpcModeRequest:
                        bool req = Convert.ToBoolean(value);
                        if (req)
                        {
                            // 原廠 4-1 規範：只有在機台待機 (3: Idle) 時才可以切換模式！
                            if (StatusCode == MachineStatus.Idle)
                            {
                                ChangeOpcModeAck = 1;
                                Log($"[模式切換成功] 上位機要求切換模式，機台確認待機中 (Idle)，Ack = 1。");
                            }
                            else
                            {
                                ChangeOpcModeAck = -1; // -1: 資料修改失敗 (自動運轉中、連續運轉中)
                                Log($"[模式切換拒絕] 機台目前狀態為 {StatusCode} (非 Idle)，拒絕切換，Ack = -1。");
                            }
                        }
                        else
                        {
                            ChangeOpcModeAck = 0;
                        }
                        NotifyStateChanged();
                        return true;

                    // 3. 配方寫入
                    case LaserOpcNode.RecipeProgramFile:
                        RequestedProgramFile = value?.ToString() ?? string.Empty;
                        Log($"[上位機寫入配方] 加工程式檔名: {RequestedProgramFile}");
                        NotifyStateChanged();
                        return true;

                    case LaserOpcNode.RecipeConditionFile:
                        RequestedConditionFile = value?.ToString() ?? "*****";
                        Log($"[上位機寫入配方] 加工條件檔名: {RequestedConditionFile}");
                        NotifyStateChanged();
                        return true;

                    case LaserOpcNode.RecipeSheetNum:
                        RequestedSheetNum = Convert.ToInt16(value);
                        Log($"[上位機寫入配方] 加工片數: {RequestedSheetNum}");
                        NotifyStateChanged();
                        return true;

                    case LaserOpcNode.GetRecipeAck:
                        short ack = Convert.ToInt16(value);
                        GetRecipeAck = ack;
                        Log($"[上位機回覆配方] GetRecipe.Ack = {ack}");

                        if (ack == 1)
                        {
                            ActiveLotId = !string.IsNullOrEmpty(RemoteLotId) ? RemoteLotId : "LOT-" + DateTime.Now.ToString("MMdd-HHmm");
                            ActiveProgramFile = RequestedProgramFile;
                            ActiveConditionFile = RequestedConditionFile;
                            ScheduledCount = RequestedSheetNum > 0 ? RequestedSheetNum : (short)5;
                            ProcessedCount = 0;
                            UnProcessedCount = 0;
                            GetRecipeRequest = false; // 機台復歸 Req

                            SetMachineStatus(MachineStatus.Ready);
                            Log($"[配方交握完成] 機台成功載入配方: {ActiveProgramFile}，批號: {ActiveLotId}，總片數: {ScheduledCount}，狀態變更為 Ready。");
                        }
                        else if (ack == -1)
                        {
                            Log($"[配方交握失敗] 上位機拒絕核發配方 (Ack = -1)。");
                            GetRecipeRequest = false;
                        }

                        NotifyStateChanged();
                        return true;

                    // 4. 連續運轉控制
                    case LaserOpcNode.StartScheduleRequest:
                        if (Convert.ToBoolean(value))
                        {
                            StartSchedule();
                        }
                        else
                        {
                            StopSchedule();
                        }
                        return true;

                    default:
                        return false;
                }
            }
        }

        #endregion

        #region 計時器核心邏輯

        private void WatchDogTick(object state)
        {
            lock (_lock)
            {
                // 1. 若未啟用 WatchDog 監控，維持滿格不倒數
                if (!IsWatchDogEnabled)
                {
                    WatchDogCountdownSec = 10;
                    return;
                }

                // 2. 若機台處於離線 (Offline) 模式，上位機心跳不監控
                if (OpcMode == MachineOperatingMode.Offline)
                {
                    WatchDogCountdownSec = 10;
                    return;
                }

                // 3. 若尚未收到過上位機第 1 次心跳 (未連線/待命中)，維持待命不倒數亦不逾時報警
                if (!HasReceivedFirstHeartbeat)
                {
                    WatchDogCountdownSec = 10;
                    return;
                }

                if (WatchDogCountdownSec > 0)
                {
                    WatchDogCountdownSec--;
                }
                else
                {
                    if (!IsWatchDogTimeout)
                    {
                        IsWatchDogTimeout = true;
                        Log("[WatchDog 逾時警告] 上位機超過 10 秒未累加 WatchDog 心跳，機台通訊異常！");
                        TriggerAlarm(9999, "Host WatchDog 通訊逾時 (10s)", "A");
                    }
                }
            }
            NotifyStateChanged();
        }

        private void ProcessingTick(object state)
        {
            lock (_lock)
            {
                if (StatusCode == MachineStatus.Running)
                {
                    ActivePower = 5500 + _rand.Next(-50, 50);
                    ActiveFrequency = 100 + _rand.Next(-2, 2);
                    ActivePulseWidth = 49.0f + (float)(_rand.NextDouble() * 1.5 - 0.75);
                    TableAdsorbPressure = -10.5f + (float)(_rand.NextDouble() * 0.4 - 0.2);
                    LensTemp = 24.2f + (float)(_rand.NextDouble() * 0.6 - 0.3);
                    CurrentCycleTime = Math.Round(2.3 + _rand.NextDouble() * 0.4, 2);

                    if (ProcessedCount < ScheduledCount)
                    {
                        ProcessedCount++;
                        Log($"[加工進行中] 片數進度: {ProcessedCount}/{ScheduledCount}，單片週期: {CurrentCycleTime}s，雷射功率: {ActivePower}W");

                        if (ProcessedCount >= ScheduledCount)
                        {
                            Log($"[批次加工完成] 批號 {ActiveLotId} 已全數加工完畢！機台轉入 Idle。");
                            SetMachineStatus(MachineStatus.Idle);
                        }
                    }
                    else
                    {
                        SetMachineStatus(MachineStatus.Idle);
                    }

                    NotifyStateChanged();
                }
            }
        }

        private void UpdateLamps()
        {
            switch (StatusCode)
            {
                case MachineStatus.Running:
                    WarningLampGreen = true;
                    WarningLampYellow = true; // 雷射射出中
                    WarningLampRed = false;
                    break;
                case MachineStatus.Ready:
                    WarningLampGreen = true;
                    WarningLampYellow = false;
                    WarningLampRed = false;
                    break;
                case MachineStatus.MachineDown:
                    WarningLampGreen = false;
                    WarningLampYellow = false;
                    WarningLampRed = true;
                    break;
                case MachineStatus.Idle:
                default:
                    WarningLampGreen = false;
                    WarningLampYellow = false;
                    WarningLampRed = false;
                    break;
            }
        }

        private void Log(string message)
        {
            LogEmitted?.Invoke(message);
        }

        private void NotifyStateChanged()
        {
            StateChanged?.Invoke();
        }

        public void Dispose()
        {
            _watchDogTimer?.Dispose();
            _processingTimer?.Dispose();
        }

        #endregion
    }
}
