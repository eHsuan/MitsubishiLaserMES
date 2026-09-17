using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace MitsubishiLaserMES.Core.Logging
{
    /// <summary>
    /// 日誌生命週期維運與自動清理服務 (依據 DOC/Log機制.txt)
    /// 負責歷史日誌資料夾之自動 ZIP 壓縮封存與超期過期檔案清理
    /// </summary>
    public class LogMaintenanceService : IDisposable
    {
        private readonly LogSettings _settings;
        private readonly Timer _timer;
        private static readonly object _syncLock = new object();
        private static bool _isRunning = false;

        public LogMaintenanceService(LogSettings settings)
        {
            _settings = settings ?? new LogSettings();

            // 若啟用維運，立即於背景非同步執行一次，並排程每 6 小時執行一次
            if (_settings.Enabled)
            {
                Task.Run(() => ExecuteMaintenance());
                // 每 6 小時觸發一次 (6 * 3600 * 1000 ms)
                int intervalMs = 6 * 60 * 60 * 1000;
                _timer = new Timer(_ => ExecuteMaintenance(), null, intervalMs, intervalMs);
            }
        }

        /// <summary>
        /// 執行單次維運作業 (防重入鎖與例外防護)
        /// </summary>
        public void ExecuteMaintenance()
        {
            if (!_settings.Enabled) return;

            lock (_syncLock)
            {
                if (_isRunning) return;
                _isRunning = true;
            }

            try
            {
                string logsRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                if (!Directory.Exists(logsRoot)) return;

                DateTime now = DateTime.Today;
                DateTime compressThreshold = now.AddDays(-Math.Max(1, _settings.CompressDays));
                DateTime retentionThreshold = now.AddDays(-Math.Max(1, _settings.RetentionDays));

                // 階段一：歷史資料夾 ZIP 壓縮封存 (Compression)
                var directories = Directory.GetDirectories(logsRoot);
                foreach (var dir in directories)
                {
                    try
                    {
                        string dirName = Path.GetFileName(dir);
                        if (DateTime.TryParseExact(dirName, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dirDate))
                        {
                            if (dirDate < compressThreshold)
                            {
                                string zipPath = Path.Combine(logsRoot, $"{dirName}.zip");
                                if (File.Exists(zipPath))
                                {
                                    // 若已有相同 zip 檔但原始資料夾仍存在，先檢查 zip 是否完好
                                    if (new FileInfo(zipPath).Length > 0)
                                    {
                                        Directory.Delete(dir, true);
                                        continue;
                                    }
                                    File.Delete(zipPath);
                                }

                                ZipFile.CreateFromDirectory(dir, zipPath, CompressionLevel.Optimal, false);

                                if (File.Exists(zipPath) && new FileInfo(zipPath).Length > 0)
                                {
                                    Directory.Delete(dir, true);
                                }
                            }
                        }
                    }
                    catch
                    {
                        // 若資料夾中有檔案正被開啟，略過並交由下次排程處理
                    }
                }

                // 階段二：過期檔案淘汰刪除 (Retention)
                // 1. 清理過期的 .zip 檔案
                var zipFiles = Directory.GetFiles(logsRoot, "*.zip");
                foreach (var zipFile in zipFiles)
                {
                    try
                    {
                        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(zipFile);
                        if (DateTime.TryParseExact(fileNameWithoutExt, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime zipDate))
                        {
                            if (zipDate < retentionThreshold)
                            {
                                File.Delete(zipFile);
                            }
                        }
                    }
                    catch
                    {
                        // 略過鎖定中檔案
                    }
                }

                // 2. 清理過期的殘餘資料夾 (若有因壓縮失敗殘留且已超期)
                var remainingDirs = Directory.GetDirectories(logsRoot);
                foreach (var dir in remainingDirs)
                {
                    try
                    {
                        string dirName = Path.GetFileName(dir);
                        if (DateTime.TryParseExact(dirName, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dirDate))
                        {
                            if (dirDate < retentionThreshold)
                            {
                                Directory.Delete(dir, true);
                            }
                        }
                    }
                    catch
                    {
                        // 略過鎖定中目錄
                    }
                }
            }
            catch
            {
                // 捕捉頂層未預期例外，維運背景作業絕不影響主程式運作
            }
            finally
            {
                lock (_syncLock)
                {
                    _isRunning = false;
                }
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
