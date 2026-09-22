using System;
using System.Threading;
using System.Threading.Tasks;
using MitsubishiLaserMES.Core.Common;

namespace MitsubishiLaserMES.Core.Services.Opc
{
    public interface IOpcService : IDisposable
    {
        bool IsConnected { get; }
        bool IsVirtual { get; }
        MitsubishiMachineStatusCode CurrentStatusCode { get; }
        MachineStatusLight CurrentStatusLight { get; }
        int ProcessedCount { get; }
        int ScheduledCount { get; }
        string ActiveLotId { get; }
        string ActiveProgramFile { get; }
        MitsubishiOpcMode CurrentOpcMode { get; }

        event Action<bool> ConnectionStateChanged;
        event Action<MitsubishiOpcMode> OpcModeChanged;
        event Action<MachineStatusLight, MachineStatusLight> StatusLightChanged;
        event Action<int, int> ProcessedCountChanged; // oldVal, newVal
        event Action<string, string, bool> AlarmTriggered; // code, msg, isStart
        event Action<string> LogMessage;

        Task<bool> ConnectAsync(CancellationToken cancellationToken = default);
        Task DisconnectAsync();
        Task<bool> DeliverRecipeAsync(string recipeId, short sheetCount, CancellationToken cancellationToken = default);
        Task<bool> StartScheduleAsync(CancellationToken cancellationToken = default);
        Task<bool> StopScheduleAsync(CancellationToken cancellationToken = default);
        Task<bool> ChangeOperatingModeAsync(short mode, CancellationToken cancellationToken = default);
    }
}
