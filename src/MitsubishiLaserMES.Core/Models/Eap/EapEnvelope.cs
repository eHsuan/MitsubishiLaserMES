using System;
using Newtonsoft.Json;
using MitsubishiLaserMES.Core.Common;

namespace MitsubishiLaserMES.Core.Models.Eap
{
    /// <summary>
    /// EAP 標準訊息信封 (規範 3.1)
    /// </summary>
    public class EapEnvelope<T> where T : class
    {
        [JsonProperty("MessageId")]
        public string MessageId { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("TimestampUtc")]
        public string TimestampUtc { get; set; } = DateTimeUtils.NowIsoUtc();

        [JsonProperty("Payload")]
        public T Payload { get; set; }

        public EapEnvelope() { }

        public EapEnvelope(T payload)
        {
            Payload = payload;
        }
    }

    /// <summary>
    /// EAP 基礎 Payload 抽象類別 (包含所有訊息必填之共通欄位)
    /// </summary>
    public abstract class EapPayloadBase
    {
        [JsonProperty("TransactionID")]
        public string TransactionID { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("CMD")]
        public string CMD { get; set; } = string.Empty;

        [JsonProperty("DATE")]
        public string DATE { get; set; } = DateTimeUtils.NowEapDate();

        [JsonProperty("Machine")]
        public string Machine { get; set; } = string.Empty;
    }

    /// <summary>
    /// 通用回覆 Payload (規範 1.6 E)
    /// </summary>
    public class EapReplyPayloadBase : EapPayloadBase
    {
        [JsonProperty("RtnResult")]
        public string RtnResult { get; set; } = "PASS"; // PASS / FAIL

        [JsonProperty("RtnMsg")]
        public string RtnMsg { get; set; } = string.Empty;

        [JsonIgnore]
        public bool IsPass => string.Equals(RtnResult, "PASS", StringComparison.OrdinalIgnoreCase);
    }
}
