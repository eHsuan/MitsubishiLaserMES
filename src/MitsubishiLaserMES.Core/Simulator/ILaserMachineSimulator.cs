using System;
using System.Collections.Generic;
using MitsubishiLaserOpc.Enums;
using MitsubishiLaserOpc.Nodes;

namespace MitsubishiLaserMES.Core.Simulator
{
    public interface ILaserMachineSimulator : IDisposable
    {
        MachineOperatingMode OpcMode { get; }
        MachineStatus StatusCode { get; }
        bool WarningLampGreen { get; }
        bool WarningLampYellow { get; }
        bool WarningLampRed { get; }

        ushort LastHostWatchDog { get; }
        int WatchDogCountdownSec { get; }
        bool IsWatchDogTimeout { get; }

        string RemoteLotId { get; }
        bool GetRecipeRequest { get; }
        string RequestedProgramFile { get; }
        string RequestedConditionFile { get; }
        short RequestedSheetNum { get; }
        short GetRecipeAck { get; }

        string ActiveLotId { get; }
        string ActiveProgramFile { get; }
        string ActiveConditionFile { get; }
        short ScheduledCount { get; }
        short ProcessedCount { get; }
        short UnProcessedCount { get; }
        double CurrentCycleTime { get; }

        int ActivePower { get; }
        int ActiveFrequency { get; }
        float ActivePulseWidth { get; }
        float TableAdsorbPressure { get; }
        float LensTemp { get; }

        IReadOnlyList<SimulatorAlarmItem> Alarms { get; }

        event Action StateChanged;
        event Action<string> LogEmitted;

        void OperatorInputLot(string lotId);
        bool TriggerAlarm(long alarmNo, string message, string type = "A");
        void ResetAlarm(int slotIndex = -1);
        bool StartSchedule();
        void StopSchedule();
        void SetMachineStatus(MachineStatus status);
        void SetOperatingMode(MachineOperatingMode mode);
        void SimulateTargetFailure();

        object HandleHostRead(NodeDescriptor node);
        bool HandleHostWrite(NodeDescriptor node, object value);
    }
}
