using System;
using System.Threading;
using System.Threading.Tasks;
using MitsubishiLaserMES.Core.Models.Eap;

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

        event Action<RemoteCommandReqPayload> RemoteCommandReceived;
        event Action<TerminalDisplayReqPayload> TerminalDisplayReceived;
        event Action<string> TimeCalibrationReceived;

        Task<bool> ConnectAsync(CancellationToken cancellationToken = default);
        Task DisconnectAsync();

        /// <summary>
        /// 發送 Request 並以 TransactionID 等待 EAP Reply，具備 T1 逾時機制 (預設 30 秒)
        /// </summary>
        Task<TReply> SendRequestAsync<TReq, TReply>(TReq reqPayload, CancellationToken cancellationToken = default)
            where TReq : EapPayloadBase
            where TReply : EapReplyPayloadBase, new();

        /// <summary>
        /// 單向發送上報訊息至 Report 主題 (可選斷線時本機暫存)
        /// </summary>
        Task<bool> PublishReportAsync<T>(T payload, bool bufferIfOffline = true)
            where T : EapPayloadBase;
    }
}
