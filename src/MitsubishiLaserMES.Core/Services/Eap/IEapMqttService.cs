using System;
using System.Threading;
using System.Threading.Tasks;
using MitsubishiLaserMES.Core.Models.Eap;
using Protocol.Core.Base;
using Protocol.Core.Messages;

namespace MitsubishiLaserMES.Core.Services.Eap
{
    public interface IEapMqttService : IDisposable
    {
        bool IsConnected { get; }
        bool IsAliveGreen { get; }

        event Action<bool> ConnectionStateChanged;
        event Action<bool> AliveStatusChanged;
        event Action<string, string> MessageReceivedLog; // topic, payload
        event Action<string, string> MessageSentLog;     // topic, payload
        event Action<string> LogMessage;

        event Action<RemoteCMDMessage> RemoteCommandReceived;
        event Action<TerminalDisplayMessage> TerminalDisplayReceived;
        event Action<string> TimeCalibrationReceived;

        /// <summary>
        /// 遠端指令非同步處理器 (支援在交握機台配方後再回覆 EAP ReplyRemoteCMD)
        /// </summary>
        Func<RemoteCMDMessage, Task<ReplyRemoteCMDMessage>> RemoteCommandHandler { get; set; }

        Task<bool> ConnectAsync(CancellationToken cancellationToken = default);
        Task DisconnectAsync();

        /// <summary>
        /// 發送 Protocol.Core Request 並以 TransactionID 等待 EAP Reply，具備 T1 逾時機制
        /// </summary>
        Task<TReply> SendProtocolRequestAsync<TReq, TReply>(TReq reqPayload, CancellationToken cancellationToken = default)
            where TReq : BaseMessage
            where TReply : ReplyBase, new();

        /// <summary>
        /// 單向發送 Protocol.Core 上報訊息至 Report 主題
        /// </summary>
        Task<bool> PublishProtocolReportAsync<T>(T payload, bool bufferIfOffline = true)
            where T : BaseMessage;

        /// <summary>
        /// 發送自定義 Request 並以 TransactionID 等待 EAP Reply，具備 T1 逾時機制 (預設 30 秒)
        /// </summary>
        Task<TReply> SendRequestAsync<TReq, TReply>(TReq reqPayload, CancellationToken cancellationToken = default)
            where TReq : EapPayloadBase
            where TReply : EapReplyPayloadBase, new();

        /// <summary>
        /// 單向發送自定義上報訊息至 Report 主題 (可選斷線時本機暫存)
        /// </summary>
        Task<bool> PublishReportAsync<T>(T payload, bool bufferIfOffline = true)
            where T : EapPayloadBase;
    }
}

