using System.Collections.Generic;
using Newtonsoft.Json;

namespace MitsubishiLaserMES.Core.Models.Eap
{
    public class NgItem
    {
        [JsonProperty("NGCode")]
        public string NGCode { get; set; } = string.Empty;

        [JsonProperty("NGChineseName")]
        public string NGChineseName { get; set; } = string.Empty;

        [JsonProperty("Qty")]
        public int Qty { get; set; } = 0;
    }

    /// <summary>
    /// 工單出站請求 (規範 3.4)
    /// </summary>
    public class TrackOutReqPayload : EapPayloadBase
    {
        public TrackOutReqPayload()
        {
            CMD = "TrackOutReq";
        }

        [JsonProperty("MaterialID")]
        public string MaterialID { get; set; } = string.Empty;

        [JsonProperty("CassetteID")]
        public List<string> CassetteID { get; set; } = new List<string>();

        [JsonProperty("WorkOrder")]
        public List<string> WorkOrder { get; set; } = new List<string>();

        [JsonProperty("UserID")]
        public string UserID { get; set; } = string.Empty;

        [JsonProperty("ToolingID")]
        public string ToolingID { get; set; } = string.Empty;

        [JsonProperty("PanelList")]
        public List<string> PanelList { get; set; } = new List<string>();

        [JsonProperty("Qty")]
        public string Qty { get; set; } = "0";

        [JsonProperty("Result")]
        public string Result { get; set; } = "PASS"; // PASS / FAIL

        [JsonProperty("NGCode")]
        public string NGCode { get; set; } = string.Empty;

        [JsonProperty("NgDetails", NullValueHandling = NullValueHandling.Ignore)]
        public List<NgItem> NgDetails { get; set; } = new List<NgItem>();
    }

    /// <summary>
    /// 工單出站回覆 (規範 3.4)
    /// </summary>
    public class ReplyTrackOutReqPayload : EapReplyPayloadBase
    {
        public ReplyTrackOutReqPayload()
        {
            CMD = "ReplyTrackOutReq";
        }

        [JsonProperty("CassetteID")]
        public string CassetteID { get; set; } = string.Empty;

        [JsonProperty("MaterialID")]
        public string MaterialID { get; set; } = string.Empty;

        [JsonProperty("WorkOrder")]
        public string WorkOrder { get; set; } = string.Empty;

        [JsonProperty("UserID")]
        public string UserID { get; set; } = string.Empty;

        [JsonProperty("ToolingID")]
        public string ToolingID { get; set; } = string.Empty;

        [JsonProperty("PanelID")]
        public string PanelID { get; set; } = string.Empty;
    }
}
