using System;

namespace MitsubishiLaserMES.Core.Common
{
    /// <summary>
    /// 機台 13 種狀態燈號定義 (參照規範 1.7)
    /// </summary>
    public enum MachineStatusLight
    {
        AutoRunning = 0,       // 綠：自動運行 (正常產出下的稼動)
        Alarm = 1,             // 紅：報警 (設備故障警報)
        Pause = 2,             // 黃：暫停 (設備無異常，手動暫停中)
        Standby = 3,           // 黃：待機 (設備無異常，未進入自動運行或 By Pass)
        WaitingForMaterial = 4,// 黃：待料 (前站未有產品流出)
        FullMaterial = 5,      // 黃：滿料 (後站未允許出料)
        LowLevelWarning = 6,   // 綠：低位預警 (即將缺料)
        LineChange = 7,        // 黃：換線 (配方程式或機構作業更換)
        OutOfMaterial = 8,     // 黃：缺料 (供料模組缺料)
        WaitingToStart = 9,    // 黃：待啟動 (異常排除後尚未恢復)
        SafetyStop = 10,       // 紅：安全停機 (安全門/光柵/急停觸發)
        QualityStop = 11,      // 紅：品質停機 (品質檢測停線/來料不良)
        Tuning = 12            // 黃：調機 (保養/點檢/調機/換零件)
    }

    /// <summary>
    /// 三菱雷射加工機狀態碼 (參照 OPC SPEC 4-3 MACHINE.Signal.Status.001.Code)
    /// </summary>
    public enum MitsubishiMachineStatusCode
    {
        MachineDown = 0, // 灰色: Ready key off 或發振器未啟動
        Ready = 1,       // 黃色: 配方正常載入待命中
        Running = 2,     // 綠色: 連續運轉中、自動運轉中
        Idle = 3,        // 黃色: 閒置待機中
        Maintain = 4,    // 藍色: 保養模式中
        Stop = 5         // 紅色: 錯誤停止、緊急停止
    }

    /// <summary>
    /// 機台 OPC 運作模式 (參照 OPC SPEC 4-1 MACHINE.Status.Opc.Mode)
    /// </summary>
    public enum MitsubishiOpcMode
    {
        Offline = 0,
        OnlineLocal = 1,
        OnlineSemiAuto = 2,
        OnlineAuto = 3
    }

    /// <summary>
    /// 警報等級
    /// </summary>
    public enum AlarmLevel
    {
        Warning, // W
        Alarm    // A
    }
}
