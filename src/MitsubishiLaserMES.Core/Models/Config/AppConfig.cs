using System;
using MitsubishiLaserMES.Core.Logging;

namespace MitsubishiLaserMES.Core.Models.Config
{
    public class MqttSettings
    {
        public string Server { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 1883;
        public string ClientId { get; set; } = "MitsubishiLaser_Middleware";
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // 站點參數
        public string Factory { get; set; } = "TN2";
        public string Floor { get; set; } = "Floor2";
        public string Area { get; set; } = "Yellow";
        public string EqID { get; set; } = "PL001";

        // 逾時設定 (毫秒，預設 30 秒)
        public int TimeoutT1Ms { get; set; } = 30000;

        // 心跳間隔 (秒，預設 30 秒)
        public int AliveCheckIntervalSec { get; set; } = 30;

        // Topic 產生輔助
        public string ReportTopic => $"{Factory}/{Floor}/{Area}/{EqID}/Report";
        public string CommandTopic => $"{Factory}/{Floor}/{Area}/{EqID}/Command";
    }

    public class OpcSettings
    {
        public bool UseVirtualSimulator { get; set; } = true;
        public string EndpointUrl { get; set; } = "opc.tcp://127.0.0.1:4840";
        public bool UseSecurity { get; set; } = false;
        public int WatchdogIntervalSec { get; set; } = 1;
        public int PollingIntervalMs { get; set; } = 1000;
    }

    public class AppConfig
    {
        public MqttSettings Mqtt { get; set; } = new MqttSettings();
        public OpcSettings Opc { get; set; } = new OpcSettings();
        public LogSettings LogSettings { get; set; } = new LogSettings();
        public string Language { get; set; } = "zh-TW"; // zh-TW 或 en-US
    }
}

