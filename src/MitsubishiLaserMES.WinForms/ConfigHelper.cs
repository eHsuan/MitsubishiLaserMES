using System;
using System.IO;
using Newtonsoft.Json;
using MitsubishiLaserMES.Core.Models.Config;

namespace MitsubishiLaserMES.WinForms
{
    public static class ConfigHelper
    {
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

        public static AppConfig LoadConfig()
        {
            AppConfig cfg = null;
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    cfg = JsonConvert.DeserializeObject<AppConfig>(json);
                }
            }
            catch { }

            if (cfg == null)
            {
                cfg = new AppConfig();
                SaveConfig(cfg);
            }

            // 載入機台字典設定檔 (Dictionaries/DR0026.json)
            cfg.Profile = LoadEquipmentProfile(cfg.EquipmentProfilePath);

            return cfg;
        }

        public static EquipmentProfile LoadEquipmentProfile(string relativeOrFullPath)
        {
            try
            {
                string targetPath = ResolveProfilePath(relativeOrFullPath);
                if (!string.IsNullOrEmpty(targetPath) && File.Exists(targetPath))
                {
                    string json = File.ReadAllText(targetPath);
                    var profile = JsonConvert.DeserializeObject<EquipmentProfile>(json);
                    if (profile != null) return profile;
                }
            }
            catch { }

            return new EquipmentProfile();
        }

        private static string ResolveProfilePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) path = "Dictionaries/DR0026.json";

            if (Path.IsPathRooted(path) && File.Exists(path)) return path;

            // 1. 檢查 AppDomain 執行目錄
            string p1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            if (File.Exists(p1)) return p1;

            // 2. 檢查 Dictionaries 子目錄 (若傳入單純檔案名稱)
            string p2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dictionaries", Path.GetFileName(path));
            if (File.Exists(p2)) return p2;

            // 3. 檢查開發環境專案根目錄回溯
            string p3 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", path);
            if (File.Exists(p3)) return Path.GetFullPath(p3);

            string p4 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", path);
            if (File.Exists(p4)) return Path.GetFullPath(p4);

            return p1;
        }

        public static void SaveConfig(AppConfig config)
        {
            try
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
            }
            catch { }
        }
    }
}
