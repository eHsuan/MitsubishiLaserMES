using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace MitsubishiLaserMES.Core.Services.Coordination
{
    /// <summary>
    /// 已進站工單本地持久化儲存服務 (重啟自動還原與中斷防護)
    /// </summary>
    public class TrackedOrderStorageService
    {
        private readonly string _filePath;
        private readonly object _lock = new object();

        public TrackedOrderStorageService(string filePath = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string dataDir = Path.Combine(baseDir, "data");
                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);
                }
                _filePath = Path.Combine(dataDir, "tracked_orders.json");
            }
            else
            {
                _filePath = filePath;
                string dir = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
        }

        /// <summary>
        /// 載入本機已進站工單快取清單
        /// </summary>
        public List<TrackedInOrderInfo> Load()
        {
            lock (_lock)
            {
                try
                {
                    if (!File.Exists(_filePath))
                    {
                        return new List<TrackedInOrderInfo>();
                    }

                    string json = File.ReadAllText(_filePath, Encoding.UTF8);
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return new List<TrackedInOrderInfo>();
                    }

                    return JsonConvert.DeserializeObject<List<TrackedInOrderInfo>>(json) ?? new List<TrackedInOrderInfo>();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[TrackedOrderStorage] 載入已進站工單失敗: {ex.Message}");
                    return new List<TrackedInOrderInfo>();
                }
            }
        }

        /// <summary>
        /// 儲存已進站工單快取清單至本機 (原子化寫入防止斷電損毀)
        /// </summary>
        public void Save(IEnumerable<TrackedInOrderInfo> orders)
        {
            lock (_lock)
            {
                try
                {
                    string dir = Path.GetDirectoryName(_filePath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    string tempPath = _filePath + ".tmp";
                    string json = JsonConvert.SerializeObject(orders ?? new List<TrackedInOrderInfo>(), Formatting.Indented);
                    File.WriteAllText(tempPath, json, Encoding.UTF8);

                    if (File.Exists(_filePath))
                    {
                        File.Delete(_filePath);
                    }
                    File.Move(tempPath, _filePath);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[TrackedOrderStorage] 儲存已進站工單失敗: {ex.Message}");
                }
            }
        }
    }
}
