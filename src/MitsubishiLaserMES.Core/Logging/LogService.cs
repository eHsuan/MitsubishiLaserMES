using System;
using System.IO;

namespace MitsubishiLaserMES.Core.Logging
{
    /// <summary>
    /// 依據 DOC/Log機制.txt 規範實作之高效、執行緒安全日誌服務
    /// </summary>
    public class LogService : ILogService
    {
        private static readonly object _logLock = new object();
        public static LogService Instance { get; } = new LogService();

        public LogSettings Settings { get; private set; }
        public string DefaultEquipmentId { get; set; } = "PL001";

        public event Action<DateTime, LogLevel, string, string, string> LogReceived;

        public LogService(LogSettings settings = null)
        {
            Settings = settings ?? new LogSettings();
        }

        public void UpdateSettings(LogSettings settings)
        {
            if (settings != null)
            {
                Settings = settings;
            }
        }

        public void Log(LogLevel level, string eventName, string message, string category = null)
        {
            var timestamp = DateTime.Now;
            string targetCategory = string.IsNullOrWhiteSpace(category)
                ? (DefaultEquipmentId ?? "System")
                : category.Trim();

            // 1. 觸發 C# 事件供 UI 即時推播
            try
            {
                LogReceived?.Invoke(timestamp, level, eventName, message, targetCategory);
            }
            catch { }

            // 2. 磁碟寫入 (利用物件鎖確保多執行緒寫入檔案時不衝突)
            try
            {
                lock (_logLock)
                {
                    string logDir = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "logs",
                        timestamp.ToString("yyyyMMdd"),
                        targetCategory
                    );

                    if (!Directory.Exists(logDir))
                    {
                        Directory.CreateDirectory(logDir);
                    }

                    int maxMB = Settings?.MaxFileSizeMB > 0 ? Settings.MaxFileSizeMB : 20;
                    string filePath = GetLogFilePath(logDir, timestamp, maxMB);

                    string evtPart = string.IsNullOrWhiteSpace(eventName) ? "" : $" [{eventName}]";
                    string logLine = $"[{timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{level}]{evtPart} : {message}{Environment.NewLine}";

                    File.AppendAllText(filePath, logLine);
                }
            }
            catch
            {
                // 防禦性設計：日誌寫入失敗不影響主程序運作
            }
        }

        public void Debug(string eventName, string message, string category = null)
            => Log(LogLevel.Debug, eventName, message, category);

        public void Info(string eventName, string message, string category = null)
            => Log(LogLevel.Info, eventName, message, category);

        public void Warn(string eventName, string message, string category = null)
            => Log(LogLevel.Warn, eventName, message, category);

        public void Error(string eventName, string message, string category = null, Exception ex = null)
        {
            string fullMsg = ex == null ? message : $"{message} | Exception: {ex.Message} -> {ex.StackTrace}";
            Log(LogLevel.Error, eventName, fullMsg, category);
        }

        public void Fatal(string eventName, string message, string category = null, Exception ex = null)
        {
            string fullMsg = ex == null ? message : $"{message} | Exception: {ex.Message} -> {ex.StackTrace}";
            Log(LogLevel.Fatal, eventName, fullMsg, category);
        }

        /// <summary>
        /// 計算單一小時之日誌檔案路徑，支援超出大小上限時自動生成滾動分卷檔案 (如 08.txt, 08_1.txt ...)
        /// </summary>
        public static string GetLogFilePath(string logDir, DateTime timestamp, int maxFileSizeMB)
        {
            long maxBytes = (long)maxFileSizeMB * 1024 * 1024;
            string baseFileName = timestamp.ToString("HH");
            string mainPath = Path.Combine(logDir, $"{baseFileName}.txt");

            if (!File.Exists(mainPath))
            {
                return mainPath;
            }

            var mainInfo = new FileInfo(mainPath);
            if (mainInfo.Length < maxBytes)
            {
                return mainPath;
            }

            // 主檔已超過限制，尋找可用滾動分卷 _1, _2 ...
            int index = 1;
            while (true)
            {
                string rollingPath = Path.Combine(logDir, $"{baseFileName}_{index}.txt");
                if (!File.Exists(rollingPath))
                {
                    return rollingPath;
                }

                var rollingInfo = new FileInfo(rollingPath);
                if (rollingInfo.Length < maxBytes)
                {
                    return rollingPath;
                }

                index++;
            }
        }
    }
}
