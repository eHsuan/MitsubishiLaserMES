using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace MitsubishiLaserOpc.Enums
{

/// <summary>
/// Stable logical identifiers for non-indexed OPC UA nodes.
/// The actual OPC UA NodeId is intentionally held by the node catalog/configuration.
/// </summary>
public enum LaserOpcNode
{
    MachineOpcMode,
    ChangeOpcModeRequest,
    RequestedOpcMode,
    ChangeOpcModeAck,
    HostWatchDog,
    RemoteLotId,
    GetRecipeRequest,
    RecipeProgramFile,
    RecipeConditionFile,
    RecipeSheetNum,
    GetRecipeAck,
    StartScheduleRequest,
    WarningLampGreen,
    WarningLampRed,
    WarningLampYellow,
    MachineStatusCode,
    MachineType,
    MachineNumber,
    OperatorId,
    ActiveProgramFile,
    ActiveConditionFile,
    ScheduledCount,
    ProcessedCount,
    UnProcessedCount,
    ActiveLotId,
    SelectedProgramFile,
    SelectedConditionFile,
    SelectedConditionNo,
    ActiveMaskNo,
    ActiveMaskDia,
    ActiveTaperNo,
    ActiveConditionGroup,
    ActiveBc,
    ActivePower,
    ActiveFrequency,
    ActivePulseWidth,
    ActivePulseStage,
    ActivePulseCount,
    ActiveReferenceEnergy,
    ActiveTolerance,
    ActiveCollimationNo,
    ActiveReferenceCollimationL,
    ActiveCurrentCollimationL,
    ActiveReferenceCollimationR,
    ActiveCurrentCollimationR,
    ActiveLightPathNo,
    TableAdsorbPressure,
    DustCollectorAdsorbPressure,
    FthetaLensStageRTemp,
    FthetaLensStageLTemp,
}
}
