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
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    var cfg = JsonConvert.DeserializeObject<AppConfig>(json);
                    if (cfg != null) return cfg;
                }
            }
            catch { }

            var defaultCfg = new AppConfig();
            SaveConfig(defaultCfg);
            return defaultCfg;
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
