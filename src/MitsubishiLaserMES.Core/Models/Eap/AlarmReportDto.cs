using Newtonsoft.Json;

namespace MitsubishiLaserMES.Core.Models.Eap
{
    /// <summary>
    /// 警報上報請求 (規範 3.5)
    /// </summary>
    public class AlarmReportPayload : EapPayloadBase
    {
        public AlarmReportPayload()
        {
            CMD = "AlarmReport";
        }

        [JsonProperty("AlarmStatus")]
        public string AlarmStatus { get; set; } = "Start"; // Start / End

        [JsonProperty("AlarmCode")]
        public string AlarmCode { get; set; } = string.Empty;

        [JsonProperty("AlarmMsg")]
        public string AlarmMsg { get; set; } = string.Empty;

        [JsonProperty("AlarmType")]
        public string AlarmType { get; set; } = "A"; // A (Alarm) / W (Warning)
    }

    /// <summary>
    /// 警報上報回覆 (規範 3.5)
    /// </summary>
    public class ReplyAlarmPayload : EapReplyPayloadBase
    {
        public ReplyAlarmPayload()
        {
            CMD = "ReplyALARM";
        }
    }
}
