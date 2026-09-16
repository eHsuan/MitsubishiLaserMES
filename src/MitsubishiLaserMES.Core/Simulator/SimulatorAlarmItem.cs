using System;

namespace MitsubishiLaserMES.Core.Simulator
{
    /// <summary>
    /// 模擬器內部維護的警報項目資料結構 (對應機台 Slot 000~009)
    /// </summary>
    public class SimulatorAlarmItem
    {
        public int SlotIndex { get; set; }
        public long AlarmNo { get; set; }
        public string AlarmMessage { get; set; } = string.Empty;
        public string AlarmType { get; set; } = "A"; // 原廠定義通常為 "A" (Alarm) 或 "W" (Warning)
        public DateTime TriggerTime { get; set; }
        public bool IsActive { get; set; }
    }
}
