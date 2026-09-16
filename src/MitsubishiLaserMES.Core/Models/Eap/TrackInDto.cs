using System.Collections.Generic;
using Newtonsoft.Json;

namespace MitsubishiLaserMES.Core.Models.Eap
{
    /// <summary>
    /// 工單進站請求 (規範 3.3)
    /// </summary>
    public class TrackInReqPayload : EapPayloadBase
    {
        public TrackInReqPayload()
        {
            CMD = "TrackInReq";
        }

        [JsonProperty("MaterialID")]
        public string MaterialID { get; set; } = string.Empty;

        [JsonProperty("CassetteID")]
        public string CassetteID { get; set; } = string.Empty;

        [JsonProperty("WorkOrder")]
        public string WorkOrder { get; set; } = string.Empty;

        [JsonProperty("UserID")]
        public string UserID { get; set; } = string.Empty;

        [JsonProperty("ToolingID")]
        public string ToolingID { get; set; } = string.Empty;

        [JsonProperty("PanelID")]
        public string PanelID { get; set; } = string.Empty;

        [JsonProperty("Qty")]
        public string Qty { get; set; } = "0";

        [JsonProperty("RecipeID", NullValueHandling = NullValueHandling.Ignore)]
        public string RecipeID { get; set; } = string.Empty;

        [JsonProperty("BatchNo", NullValueHandling = NullValueHandling.Ignore)]
        public string BatchNo { get; set; } = string.Empty;
    }

    /// <summary>
    /// 工單進站回覆 (規範 3.3)
    /// </summary>
    public class ReplyTrackInReqPayload : EapReplyPayloadBase
    {
        public ReplyTrackInReqPayload()
        {
            CMD = "ReplyTrackInReq";
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

        // 額外資訊 (支援帶回料號、製程等)
        [JsonProperty("PartNo", NullValueHandling = NullValueHandling.Ignore)]
        public string PartNo { get; set; } = string.Empty;

        [JsonProperty("ProcessNo", NullValueHandling = NullValueHandling.Ignore)]
        public string ProcessNo { get; set; } = string.Empty;

        [JsonProperty("ProcessName", NullValueHandling = NullValueHandling.Ignore)]
        public string ProcessName { get; set; } = string.Empty;

        [JsonProperty("RecipeID", NullValueHandling = NullValueHandling.Ignore)]
        public string RecipeID { get; set; } = string.Empty;

        [JsonProperty("BatchNo", NullValueHandling = NullValueHandling.Ignore)]
        public string BatchNo { get; set; } = string.Empty;
    }
}
