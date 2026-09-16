using Newtonsoft.Json;

namespace MitsubishiLaserMES.Core.Models.Eap
{
    /// <summary>
    /// 機台狀態/燈號變更上報請求 (規範 3.7)
    /// </summary>
    public class StatusChangeReportPayload : EapPayloadBase
    {
        public StatusChangeReportPayload()
        {
            CMD = "StatusChangeReport";
        }

        [JsonProperty("OldStatus")]
        public string OldStatus { get; set; } = "3"; // 預設 3:待機

        [JsonProperty("NewStatus")]
        public string NewStatus { get; set; } = "0"; // 燈號代碼 0~12
    }

    /// <summary>
    /// 機台狀態/燈號變更上報回覆 (規範 3.7)
    /// </summary>
    public class ReplyStatusChangeReportPayload : EapReplyPayloadBase
    {
        public ReplyStatusChangeReportPayload()
        {
            CMD = "ReplyStatusChangeReport";
        }
    }
}
