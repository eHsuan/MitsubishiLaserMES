using System.Collections.Generic;
using Newtonsoft.Json;

namespace MitsubishiLaserMES.Core.Models.Eap
{
    public class EventReportItem
    {
        [JsonProperty("Parameter")]
        public string Parameter { get; set; } = string.Empty;

        [JsonProperty("Value")]
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// 人員登入驗證請求 (規範 3.2)
    /// </summary>
    public class UserVerifyReqPayload : EapPayloadBase
    {
        public UserVerifyReqPayload(string eventId = "104", string eventName = "USER_VERIFY")
        {
            CMD = "EventReport";
            EventID = string.IsNullOrWhiteSpace(eventId) ? "104" : eventId;
            EventName = string.IsNullOrWhiteSpace(eventName) ? "USER_VERIFY" : eventName;
        }

        [JsonProperty("EventID")]
        public string EventID { get; set; } = "104";

        [JsonProperty("EventName")]
        public string EventName { get; set; } = "USER_VERIFY";

        [JsonProperty("Data")]
        public List<EventReportItem> Data { get; set; } = new List<EventReportItem>();
    }

    /// <summary>
    /// 人員登入驗證回覆 (規範 3.2)
    /// </summary>
    public class UserVerifyReplyPayload : EapReplyPayloadBase
    {
        public UserVerifyReplyPayload()
        {
            CMD = "ReplyEventReport";
        }

        /// <summary>
        /// EAP 回傳之人員工號 (即 RtnMsg / RtnMessage)
        /// </summary>
        [JsonIgnore]
        public string EmployeeId => !string.IsNullOrWhiteSpace(RtnMsg) ? RtnMsg.Trim() : string.Empty;
    }
}
