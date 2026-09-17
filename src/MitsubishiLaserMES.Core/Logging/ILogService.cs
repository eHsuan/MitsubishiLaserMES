using System;

namespace MitsubishiLaserMES.Core.Logging
{
    /// <summary>
    /// 日誌服務介面，負責統一的結構化日誌記錄與 UI 事件推播
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// 當有新日誌產生時觸發 (timestamp, level, eventName, message, category)
        /// </summary>
        event Action<DateTime, LogLevel, string, string, string> LogReceived;

        /// <summary>
        /// 當前日誌組態設定
        /// </summary>
        LogSettings Settings { get; }

        /// <summary>
        /// 預設設備/模組識別代碼 (如 PL001)
        /// </summary>
        string DefaultEquipmentId { get; set; }

        /// <summary>
        /// 核心日誌寫入方法
        /// </summary>
        void Log(LogLevel level, string eventName, string message, string category = null);

        void Debug(string eventName, string message, string category = null);
        void Info(string eventName, string message, string category = null);
        void Warn(string eventName, string message, string category = null);
        void Error(string eventName, string message, string category = null, Exception ex = null);
        void Fatal(string eventName, string message, string category = null, Exception ex = null);
    }
}
