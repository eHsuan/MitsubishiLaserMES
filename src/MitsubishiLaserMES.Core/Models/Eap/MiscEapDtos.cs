using Newtonsoft.Json;

namespace MitsubishiLaserMES.Core.Models.Eap
{
    /// <summary>
    /// 連線存活檢測請求 (規範 3.8) - EQ 或 BC 主動發送
    /// </summary>
    public class AliveCheckReqPayload : EapPayloadBase
    {
        public AliveCheckReqPayload()
        {
            CMD = "AreYouThere";
        }
    }

    /// <summary>
    /// 連線存活檢測回覆 (規範 3.8)
    /// </summary>
    public class AliveCheckReplyPayload : EapReplyPayloadBase
    {
        public AliveCheckReplyPayload()
        {
            CMD = "IamHere";
            RtnResult = "PASS";
        }
    }

    /// <summary>
    /// 時間校正請求 (規範 3.9) - 由 BC 主動發送
    /// </summary>
    public class TimeCalibrationReqPayload : EapPayloadBase
    {
        public TimeCalibrationReqPayload()
        {
            CMD = "TimeCalibrate";
        }
    }

    /// <summary>
    /// 時間校正回覆 (規範 3.9) - EQ 回覆
    /// </summary>
    public class TimeCalibrationReplyPayload : EapReplyPayloadBase
    {
        public TimeCalibrationReplyPayload()
        {
            CMD = "ReplyTimeCalibrate";
            RtnResult = "PASS";
        }
    }

    /// <summary>
    /// 遠端指令請求 (規範 3.11) - 由 BC 發送: START, STOP, PP_SELECT, PAUSE, RESUME
    /// </summary>
    public class RemoteCommandReqPayload : EapPayloadBase
    {
        public RemoteCommandReqPayload()
        {
            CMD = "RemoteCMD";
        }

        [JsonProperty("RemoteCMDType")]
        public string RemoteCMDType { get; set; } = string.Empty;

        [JsonProperty("RecipeID", NullValueHandling = NullValueHandling.Ignore)]
        public string RecipeID { get; set; } = string.Empty;
    }

    /// <summary>
    /// 遠端指令回覆 (規範 3.11)
    /// </summary>
    public class RemoteCommandReplyPayload : EapReplyPayloadBase
    {
        public RemoteCommandReplyPayload()
        {
            CMD = "ReplyRemoteCMD";
        }

        [JsonProperty("RemoteCMDType")]
        public string RemoteCMDType { get; set; } = string.Empty;
    }

    /// <summary>
    /// 遠端訊息顯示請求 (規範 3.12) - 由 BC 發送
    /// </summary>
    public class TerminalDisplayReqPayload : EapPayloadBase
    {
        public TerminalDisplayReqPayload()
        {
            CMD = "TerminalDisplay";
        }

        [JsonProperty("Message")]
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// 遠端訊息顯示回覆 (規範 3.12)
    /// </summary>
    public class TerminalDisplayReplyPayload : EapReplyPayloadBase
    {
        public TerminalDisplayReplyPayload()
        {
            CMD = "ReplyTerminalDisplay";
            RtnResult = "PASS";
        }
    }
}
