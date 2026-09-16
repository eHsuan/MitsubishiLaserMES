using System.Collections.Generic;
using Newtonsoft.Json;

namespace MitsubishiLaserMES.Core.Models.Eap
{
    public class ProcessDataItem
    {
        [JsonProperty("Parameter")]
        public string Parameter { get; set; } = string.Empty;

        [JsonProperty("Value")]
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// 製程資料上報請求 (規範 3.6)
    /// </summary>
    public class ProcessDataReportPayload : EapPayloadBase
    {
        public ProcessDataReportPayload()
        {
            CMD = "ProcessDataReport";
        }

        [JsonProperty("UserID")]
        public string UserID { get; set; } = string.Empty;

        [JsonProperty("MaterialID")]
        public string MaterialID { get; set; } = string.Empty;

        [JsonProperty("CassetteID")]
        public string CassetteID { get; set; } = string.Empty;

        [JsonProperty("WorkOrder")]
        public string WorkOrder { get; set; } = string.Empty;

        [JsonProperty("RecipeID")]
        public string RecipeID { get; set; } = string.Empty;

        [JsonProperty("Result")]
        public string Result { get; set; } = "PASS"; // PASS / FAIL / NG

        [JsonProperty("ProcessTime")]
        public string ProcessTime { get; set; } = "0";

        [JsonProperty("ProcessData")]
        public List<ProcessDataItem> ProcessData { get; set; } = new List<ProcessDataItem>();
    }

    /// <summary>
    /// 製程資料上報回覆 (規範 3.6)
    /// </summary>
    public class ReplyProcessDataReportPayload : EapReplyPayloadBase
    {
        public ReplyProcessDataReportPayload()
        {
            CMD = "ReplyProcessDataReport";
        }
    }
}
