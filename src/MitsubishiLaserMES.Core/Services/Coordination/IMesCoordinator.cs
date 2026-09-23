using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MitsubishiLaserMES.Core.Common;
using MitsubishiLaserMES.Core.Models.Eap;
using MitsubishiLaserMES.Core.Services.Eap;
using MitsubishiLaserMES.Core.Services.Opc;
using Protocol.Core.Messages;

namespace MitsubishiLaserMES.Core.Services.Coordination
{
    public class TrackedInOrderInfo
    {
        public string WorkOrder { get; set; } = string.Empty;
        public string CassetteId { get; set; } = string.Empty;
        public string RecipeId { get; set; } = string.Empty;
        public string PartNo { get; set; } = string.Empty;
        public string ProcessNo { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty;
        public int TotalQty { get; set; } = 0;
        public DateTime TrackInTime { get; set; } = DateTime.Now;
    }

    public interface IMesCoordinator : IDisposable
    {
        IEapMqttService EapService { get; }
        IOpcService OpcService { get; }

        string CurrentOperatorId { get; }
        string CurrentOperatorName { get; }
        bool IsOperatorLoggedIn { get; }
        bool IsTrackedIn { get; }
        TrackedInOrderInfo CurrentOrder { get; }
        IReadOnlyList<TrackedInOrderInfo> TrackedInOrders { get; }

        event Action<string, string> OperatorLoggedIn; // id, name
        event Action OperatorLoggedOut;
        event Action<TrackedInOrderInfo> TrackInCompleted;
        event Action<string> TrackOutCompleted;
        event Action<string> TrackInRemoved;
        event Action<string> TerminalMessageNotified;
        event Action<string> SystemLogMessage;

        Task<bool> InitializeAsync(CancellationToken cancellationToken = default);
        Task<UserVerifyReplyPayload> LoginWithBarcodeAsync(string userBarcode, CancellationToken cancellationToken = default);
        void LogoutOperator();
        Task<ReplyTrackInReqMessage> TrackInAsync(TrackInReqMessage req, CancellationToken cancellationToken = default);
        Task<ReplyTrackOutReqMessage> TrackOutAsync(TrackOutReqMessage req, CancellationToken cancellationToken = default);
        Task<ReplyTrackInReqPayload> TrackInAsync(TrackInReqPayload req, CancellationToken cancellationToken = default);
        Task<ReplyTrackOutReqPayload> TrackOutAsync(TrackOutReqPayload req, CancellationToken cancellationToken = default);
        bool RemoveTrackedInOrder(string workOrder);
        Task<bool> SwitchOpcModeAsync(short mode, CancellationToken cancellationToken = default);
    }
}
