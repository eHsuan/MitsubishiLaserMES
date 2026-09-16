using System.Collections.Generic;
using Newtonsoft.Json;

namespace MitsubishiLaserMES.Core.Models.Eap
{
    /// <summary>
    /// 初始化追蹤資料設定 (規範 3.15.1) - 由 BC 發送
    /// </summary>
    public class InitTraceDataReqPayload : EapPayloadBase
    {
        public InitTraceDataReqPayload()
        {
            CMD = "InitTraceData";
        }

        [JsonProperty("Timer")]
        public int Timer { get; set; } = 6000; // 毫秒

        [JsonProperty("Enable")]
        public bool Enable { get; set; } = true;
    }

    public class ReplyInitTraceDataPayload : EapReplyPayloadBase
    {
        public ReplyInitTraceDataPayload()
        {
            CMD = "ReplyInitTraceData";
        }
    }

    /// <summary>
    /// 追蹤資料上報 (規範 3.15.2) - EQ 發送
    /// </summary>
    public class RtnTraceDataPayload : EapPayloadBase
    {
        public RtnTraceDataPayload()
        {
            CMD = "RtnTraceData";
        }

        [JsonProperty("Status")]
        public string Status { get; set; } = "0";

        [JsonProperty("CycleTime")]
        public string CycleTime { get; set; } = "0.0";

        [JsonProperty("Availability")]
        public string Availability { get; set; } = "100.0";

        [JsonProperty("Parameter")]
        public List<Dictionary<string, object>> Parameter { get; set; } = new List<Dictionary<string, object>>();

        [JsonProperty("RecipeDATA", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> RecipeDATA { get; set; }
    }

    public class ReplyRtnTraceDataPayload : EapReplyPayloadBase
    {
        public ReplyRtnTraceDataPayload()
        {
            CMD = "ReplyRtnTraceData";
        }
    }
}
