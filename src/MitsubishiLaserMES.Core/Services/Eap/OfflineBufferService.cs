using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MitsubishiLaserMES.Core.Services.Eap
{
    public class BufferedMessage
    {
        public string MessageId { get; set; } = Guid.NewGuid().ToString();
        public string Topic { get; set; }
        public string JsonContent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// 離線資料暫存與補傳服務
    /// </summary>
    public class OfflineBufferService
    {
        private readonly ConcurrentQueue<BufferedMessage> _queue = new ConcurrentQueue<BufferedMessage>();
        private readonly string _bufferFilePath;

        public int Count => _queue.Count;

        public OfflineBufferService(string storageDir = null)
        {
            storageDir ??= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BufferData");
            if (!Directory.Exists(storageDir))
            {
                Directory.CreateDirectory(storageDir);
            }
            _bufferFilePath = Path.Combine(storageDir, "offline_buffer.json");
            LoadFromDisk();
        }

        public void Enqueue(string topic, string jsonContent)
        {
            var msg = new BufferedMessage
            {
                Topic = topic,
                JsonContent = jsonContent
            };
            _queue.Enqueue(msg);
            SaveToDisk();
        }

        public bool TryDequeue(out BufferedMessage message)
        {
            bool success = _queue.TryDequeue(out message);
            if (success)
            {
                SaveToDisk();
            }
            return success;
        }

        private void SaveToDisk()
        {
            try
            {
                var list = _queue.ToArray();
                var json = JsonConvert.SerializeObject(list, Formatting.Indented);
                File.WriteAllText(_bufferFilePath, json);
            }
            catch { }
        }

        private void LoadFromDisk()
        {
            try
            {
                if (File.Exists(_bufferFilePath))
                {
                    var json = File.ReadAllText(_bufferFilePath);
                    var items = JsonConvert.DeserializeObject<List<BufferedMessage>>(json);
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            _queue.Enqueue(item);
                        }
                    }
                }
            }
            catch { }
        }
    }
}
