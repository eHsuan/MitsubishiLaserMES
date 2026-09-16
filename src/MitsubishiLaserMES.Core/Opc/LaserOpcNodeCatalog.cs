using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MitsubishiLaserOpc.Enums;

namespace MitsubishiLaserOpc.Nodes
{

public static class LaserOpcNodeCatalog
{
    private static readonly IReadOnlyDictionary<LaserOpcNode, NodeDescriptor> Nodes = Build();
    private static readonly IReadOnlyDictionary<LaserOpcNodeGroup, NodeGroupDescriptor> Groups = new Dictionary<LaserOpcNodeGroup, NodeGroupDescriptor>
    {
        [LaserOpcNodeGroup.Mask] = new NodeGroupDescriptor(LaserOpcNodeGroup.Mask, "MACHINE.Status.Mask.{index:000}.{field}", NodeDirection.MachineToHost, new[] { "Enable", "Dia" }, "Mask 001-012"),
        [LaserOpcNodeGroup.Alarm] = new NodeGroupDescriptor(LaserOpcNodeGroup.Alarm, "MACHINE.Status.Alarm.{index:000}.{field}", NodeDirection.MachineToHost, new[] { "Date", "Time", "No", "Message" }, "Alarm 000-009"),
        [LaserOpcNodeGroup.Maintenance] = new NodeGroupDescriptor(LaserOpcNodeGroup.Maintenance, "MACHINE.Status.Maint.{index:000}.{field}", NodeDirection.MachineToHost, new[] { "Name", "SettingTime", "IntegrationTime", "Date", "Count", "IntegrationPercent" }, "Maintenance"),
        [LaserOpcNodeGroup.AaaGosaLog] = new NodeGroupDescriptor(LaserOpcNodeGroup.AaaGosaLog, "MACHINE.Status.Log.aaaGosa.{stage}.{beam}.{field}", NodeDirection.MachineToHost, new[] { "Date", "Time", "MaxErrorR", "MaxErrorX", "MaxErrorY" }, "AAA error log"),
        [LaserOpcNodeGroup.PowerDataLog] = new NodeGroupDescriptor(LaserOpcNodeGroup.PowerDataLog, "MACHINE.Status.Log.pwrdata.{stage}.{beam}.{field}", NodeDirection.MachineToHost, new[] { "Date", "Time", "ConditionNo", "Power", "Frequency", "OnTime", "ReferenceEnergy", "MeasuredEnergy", "TolerableRange", "CollimationNo", "ReferenceMagnification", "CurrentMagnification", "MaskNo", "Gain", "Offset", "BeamOnTime", "BlowerOnTime", "LaserReadyOnTime", "LightPathTypeNo", "ConditionFile", "PlsStage", "RetryNo" }, "Power data log"),
        [LaserOpcNodeGroup.PowerMultiLog] = new NodeGroupDescriptor(LaserOpcNodeGroup.PowerMultiLog, "MACHINE.Status.Log.pwrmulti.{stage}.{beam}.{field}", NodeDirection.MachineToHost, new[] { "Date", "Time", "ConditionNo", "Power", "Frequency", "OnTime", "ReferenceEnergy", "MeasuredEnergy1", "MeasuredEnergy2", "MeasuredEnergy3", "MeasuredEnergy4", "MeasuredEnergy5", "MeasuredEnergy6", "MeasuredEnergy7", "MeasuredEnergy8", "MeasuredEnergy9", "MeasuredEnergy10", "MeasuredEnergy11", "MeasuredEnergy12", "MeasuredEnergy13", "MeasuredEnergy14", "MeasuredEnergy15", "MeasuredEnergy16", "MeasuredEnergy17", "CollimationNo", "ReferenceMagnification", "CurrentMagnification", "MaskNo", "Gain", "Offset", "BeamOnTime", "BlowerOnTime", "LaserReadyOnTime", "LightPathTypeNo", "ConditionFile", "PlsStage" }, "Power multi log"),
        [LaserOpcNodeGroup.M307DataLog] = new NodeGroupDescriptor(LaserOpcNodeGroup.M307DataLog, "MACHINE.Status.Log.m307data.{field}", NodeDirection.MachineToHost, new[] { "Date", "Time", "Power", "Frequency", "OnTime", "MeasuredPower", "CheckedPower", "Gain", "Offset", "BeamOnTime", "BlowerOnTime", "LaserReadyOnTime", "Status" }, "M307 data log"),
        [LaserOpcNodeGroup.M370DataLog] = new NodeGroupDescriptor(LaserOpcNodeGroup.M370DataLog, "MACHINE.Status.Log.m370data.{stage}.Camera0.{field}", NodeDirection.MachineToHost, new[] { "Date", "Time", "OffsetX", "OffsetY", "OffsetZ" }, "M370 data log"),
        [LaserOpcNodeGroup.MaskGosaLog] = new NodeGroupDescriptor(LaserOpcNodeGroup.MaskGosaLog, "MACHINE.Status.Log.maskGosa.{stage}.{field}", NodeDirection.MachineToHost, new[] { "Date", "Time", "MaximumErrorR", "MaximumErrorX", "MaximumErrorY", "MaskNo" }, "Mask error log"),
        [LaserOpcNodeGroup.ZAxisLog] = new NodeGroupDescriptor(LaserOpcNodeGroup.ZAxisLog, "MACHINE.Status.Log.Zaxis.{field}", NodeDirection.MachineToHost, new[] { "Date", "Time", "ZRAxis", "ZLAxis", "ProcessingProgram", "ScheduleState", "LoadingDirection", "SchSheet", "ProSheet", "UnProSheet" }, "Z axis log"),
        [LaserOpcNodeGroup.LengthLog] = new NodeGroupDescriptor(LaserOpcNodeGroup.LengthLog, "MACHINE.Status.Log.length.{stage}.{field}", NodeDirection.MachineToHost, new[] { "Date", "Time", "CompensationPoints", "Length15", "Length16", "Length17", "Length18", "Length19", "Length20", "Error", "PieceNo", "LengthMaxErr", "ExpansionRateMin", "ExpansionRateMax" }, "Length log"),
        [LaserOpcNodeGroup.SinsyukuLog] = new NodeGroupDescriptor(LaserOpcNodeGroup.SinsyukuLog, "MACHINE.Status.Log.sinsyuku.{stage}.{field}", NodeDirection.MachineToHost, new[] { "Date", "Time", "AdministrationNo", "SerialNo", "ExpansionRateX", "ExpansionRateY", "MainProgramFile", "ConditionFile", "Status", "Error", "OffsetX", "OffsetY", "Theta", "FeedHoldBlock", "EndBlock", "ExpansionRateSettingX", "ExpansionRateSettingY" }, "Shrinkage log"),
        [LaserOpcNodeGroup.TableLog] = new NodeGroupDescriptor(LaserOpcNodeGroup.TableLog, "MACHINE.Status.Log.table.{stage}.{field}", NodeDirection.MachineToHost, new[] { "Date", "Time", "Z", "X", "Y", "ZAverage", "ZRange", "ProcessingProgram", "ZAxis", "BaseWork", "RefWork", "ZWork" }, "Table log"),
    };
    public static IReadOnlyCollection<NodeDescriptor> All => Nodes.Values.ToArray();
    public static IReadOnlyCollection<int> MaintenanceItemIndexes { get; } = new[] { 0, 1, 2, 3, 4, 6, 7, 8, 9, 11 };
    public static NodeDescriptor Get(LaserOpcNode node) => Nodes[node];
    public static NodeGroupDescriptor GetGroup(LaserOpcNodeGroup group) => Groups[group];
    public static string FormatGroupTag(LaserOpcNodeGroup group, int index, string field)
    {
        // SPEC 4-9 prints "Setting.Time" for item 000 and "SettingTime" from item 001 on.
        // Real machine (192.168.5.3, 2026-08-26 audit CSV) answers Good for the undotted
        // "SettingTime" on every item including 000, so the dotted form is a document typo.
        return Groups[group].TagTemplate
            .Replace("{index:000}", index.ToString("000"))
            .Replace("{field}", field);
    }
    public static string FormatHistoryLogTag(LaserOpcNodeGroup group, string stage, string beam, string field)
    {
        return Groups[group].TagTemplate
            .Replace("{stage}", stage ?? string.Empty)
            .Replace("{beam}", beam ?? string.Empty)
            .Replace("{field}", field);
    }

    private static IReadOnlyDictionary<LaserOpcNode, NodeDescriptor> Build()
    {
        var nodes = ((LaserOpcNode[])Enum.GetValues(typeof(LaserOpcNode))).ToDictionary(
            node => node,
            node => new NodeDescriptor(node, $"MACHINE.Status.{node}", OpcValueType.String, NodeDirection.MachineToHost, node.ToString()));

        Add(LaserOpcNode.MachineOpcMode, "MACHINE.Status.Opc.Mode", OpcValueType.Int16, NodeDirection.MachineToHost, "目前機台 OPC 模式", new NodeConstraints { AllowedIntegerValues = new long[] { 0, 1, 2, 3 } });
        Add(LaserOpcNode.ChangeOpcModeRequest, "HOST.Status.Remote.ChangeOpcMode.Req", OpcValueType.Boolean, NodeDirection.HostToMachine, "要求切換 OPC 模式（僅 Idle）");
        Add(LaserOpcNode.RequestedOpcMode, "HOST.Status.Remote.Data.Mode", OpcValueType.Int16, NodeDirection.HostToMachine, "目標 OPC 模式（1=Local、2=Semi-Auto、3=Auto）", new NodeConstraints { AllowedIntegerValues = new long[] { 1, 2, 3 } });
        Add(LaserOpcNode.ChangeOpcModeAck, "MACHINE.Status.Remote.ChangeOpcMode.Ack", OpcValueType.Int16, NodeDirection.MachineToHost, "模式切換結果回覆");
        Add(LaserOpcNode.HostWatchDog, "HOST.Status.Opc.WatchDog", OpcValueType.UInt16, NodeDirection.HostToMachine, "Host WatchDog 計數");
        Add(LaserOpcNode.RemoteLotId, "MACHINE.Status.Remote.Data.LotID", OpcValueType.String, NodeDirection.MachineToHost, "設備送出的 Lot ID", new NodeConstraints { MaxStringLength = 20 });
        Add(LaserOpcNode.GetRecipeRequest, "MACHINE.Status.Remote.GetRecipe.Req", OpcValueType.Boolean, NodeDirection.MachineToHost, "設備要求取得 Recipe");
        Add(LaserOpcNode.RecipeProgramFile, "HOST.Status.Remote.Data.Recipe.000.ProgramFile", OpcValueType.String, NodeDirection.HostToMachine, "加工程式檔名（含路徑）", new NodeConstraints { MaxStringLength = 128 });
        Add(LaserOpcNode.RecipeConditionFile, "HOST.Status.Remote.Data.Recipe.000.ConditionFile", OpcValueType.String, NodeDirection.HostToMachine, "加工條件檔名（含路徑）", new NodeConstraints { MaxStringLength = 128 });
        Add(LaserOpcNode.RecipeSheetNum, "HOST.Status.Remote.Data.Recipe.000.SheetNum", OpcValueType.Int16, NodeDirection.HostToMachine, "預定加工片數；-1 代表不限", new NodeConstraints { Minimum = -1, Maximum = 9999 });
        Add(LaserOpcNode.GetRecipeAck, "HOST.Status.Remote.GetRecipe.Ack", OpcValueType.Int16, NodeDirection.HostToMachine, "Recipe 設定結果回覆");
        Add(LaserOpcNode.StartScheduleRequest, "HOST.Status.Remote.StartSchedule.Req", OpcValueType.Boolean, NodeDirection.HostToMachine, "自動開始連續運轉要求");
        Add(LaserOpcNode.MachineStatusCode, "MACHINE.Signal.Status.001.Code", OpcValueType.Int32, NodeDirection.MachineToHost, "機台狀態碼（1=Ready、2=Running、3=Idle）");
        Add(LaserOpcNode.WarningLampGreen, "MACHINE.Signal.WarningLamp.Green", OpcValueType.Boolean, NodeDirection.MachineToHost, "Green warning lamp");
        Add(LaserOpcNode.WarningLampYellow, "MACHINE.Signal.WarningLamp.Yellow", OpcValueType.Boolean, NodeDirection.MachineToHost, "Yellow warning lamp");
        Add(LaserOpcNode.WarningLampRed, "MACHINE.Signal.WarningLamp.Red", OpcValueType.Boolean, NodeDirection.MachineToHost, "Red warning lamp");
        Add(LaserOpcNode.MachineType, "MACHINE.Status.Info.Type", OpcValueType.String, NodeDirection.MachineToHost, "Machine type");
        Add(LaserOpcNode.MachineNumber, "MACHINE.Status.Info.No", OpcValueType.String, NodeDirection.MachineToHost, "Machine number");
        Add(LaserOpcNode.OperatorId, "MACHINE.Status.Info.Operator.ID", OpcValueType.String, NodeDirection.MachineToHost, "Operator ID");
        Add(LaserOpcNode.ActiveProgramFile, "MACHINE.Status.Schedule.Active.ProgramFile", OpcValueType.String, NodeDirection.MachineToHost, "Active program file");
        Add(LaserOpcNode.ActiveConditionFile, "MACHINE.Status.Schedule.Active.ConditionFile", OpcValueType.String, NodeDirection.MachineToHost, "Active condition file");
        Add(LaserOpcNode.ScheduledCount, "MACHINE.Status.Schedule.Active.ScheduledCount", OpcValueType.Int16, NodeDirection.MachineToHost, "Scheduled count");
        Add(LaserOpcNode.ProcessedCount, "MACHINE.Status.Schedule.Active.ProcessedCount", OpcValueType.Int16, NodeDirection.MachineToHost, "Processed count");
        Add(LaserOpcNode.UnProcessedCount, "MACHINE.Status.Schedule.Active.UnProcessedCount", OpcValueType.Int16, NodeDirection.MachineToHost, "Unprocessed count");
        Add(LaserOpcNode.ActiveLotId, "MACHINE.Status.Schedule.Active.AdminNo", OpcValueType.String, NodeDirection.MachineToHost, "Active lot ID");
        Add(LaserOpcNode.SelectedProgramFile, "MACHINE.Status.Program.FileName", OpcValueType.String, NodeDirection.MachineToHost, "Selected program file");
        Add(LaserOpcNode.SelectedConditionFile, "MACHINE.Status.Condition.FileName", OpcValueType.String, NodeDirection.MachineToHost, "Selected condition file");
        Add(LaserOpcNode.SelectedConditionNo, "MACHINE.Status.Condition.SelectedNo", OpcValueType.Int16, NodeDirection.MachineToHost, "Selected condition number");
        Add(LaserOpcNode.ActiveMaskNo, "MACHINE.Status.Condition.Active.MaskNo", OpcValueType.Int16, NodeDirection.MachineToHost, "Active mask number");
        Add(LaserOpcNode.ActiveMaskDia, "MACHINE.Status.Condition.Active.MaskDia", OpcValueType.Single, NodeDirection.MachineToHost, "Active mask diameter");
        Add(LaserOpcNode.ActiveTaperNo, "MACHINE.Status.Condition.Active.TaperNo", OpcValueType.Int16, NodeDirection.MachineToHost, "Active taper number");
        Add(LaserOpcNode.ActiveConditionGroup, "MACHINE.Status.Condition.Active.Group", OpcValueType.Int16, NodeDirection.MachineToHost, "Active condition group");
        Add(LaserOpcNode.ActiveBc, "MACHINE.Status.Condition.Active.BC", OpcValueType.String, NodeDirection.MachineToHost, "Active BC");
        Add(LaserOpcNode.ActivePower, "MACHINE.Status.Condition.Active.Power", OpcValueType.Int32, NodeDirection.MachineToHost, "Active power");
        Add(LaserOpcNode.ActiveFrequency, "MACHINE.Status.Condition.Active.Frequency", OpcValueType.Int32, NodeDirection.MachineToHost, "Active frequency");
        Add(LaserOpcNode.ActivePulseWidth, "MACHINE.Status.Condition.Active.PlsWidth", OpcValueType.Single, NodeDirection.MachineToHost, "Active pulse width");
        Add(LaserOpcNode.ActivePulseStage, "MACHINE.Status.Condition.Active.PlsStage", OpcValueType.String, NodeDirection.MachineToHost, "Active pulse stage");
        Add(LaserOpcNode.ActivePulseCount, "MACHINE.Status.Condition.Active.PlsNum", OpcValueType.Int32, NodeDirection.MachineToHost, "Active pulse count");
        Add(LaserOpcNode.ActiveReferenceEnergy, "MACHINE.Status.Condition.Active.RefEnergy", OpcValueType.Single, NodeDirection.MachineToHost, "Reference energy");
        Add(LaserOpcNode.ActiveTolerance, "MACHINE.Status.Condition.Active.Tolerance", OpcValueType.Single, NodeDirection.MachineToHost, "Tolerance");
        Add(LaserOpcNode.ActiveCollimationNo, "MACHINE.Status.Condition.Active.ColliNo", OpcValueType.Int16, NodeDirection.MachineToHost, "Collimation number");
        Add(LaserOpcNode.ActiveReferenceCollimationL, "MACHINE.Status.Condition.Active.RefColliL", OpcValueType.Single, NodeDirection.MachineToHost, "Reference collimation L");
        Add(LaserOpcNode.ActiveCurrentCollimationL, "MACHINE.Status.Condition.Active.CurColliL", OpcValueType.Single, NodeDirection.MachineToHost, "Current collimation L");
        Add(LaserOpcNode.ActiveReferenceCollimationR, "MACHINE.Status.Condition.Active.RefColliR", OpcValueType.Single, NodeDirection.MachineToHost, "Reference collimation R");
        Add(LaserOpcNode.ActiveCurrentCollimationR, "MACHINE.Status.Condition.Active.CurColliR", OpcValueType.Single, NodeDirection.MachineToHost, "Current collimation R");
        Add(LaserOpcNode.ActiveLightPathNo, "MACHINE.Status.Condition.Active.LightPathNo", OpcValueType.Int16, NodeDirection.MachineToHost, "Light path number");
        Add(LaserOpcNode.TableAdsorbPressure, "MACHINE.Status.Table.AdsorbPressure", OpcValueType.Single, NodeDirection.MachineToHost, "Table pressure");
        Add(LaserOpcNode.DustCollectorAdsorbPressure, "MACHINE.Status.DustCollector.AdsorbPressure", OpcValueType.Single, NodeDirection.MachineToHost, "Dust collector pressure");
        Add(LaserOpcNode.FthetaLensStageRTemp, "MACHINE.Status.FThetaLens.StageR.Temp", OpcValueType.Single, NodeDirection.MachineToHost, "F-theta stage R temperature");
        Add(LaserOpcNode.FthetaLensStageLTemp, "MACHINE.Status.FThetaLens.StageL.Temp", OpcValueType.Single, NodeDirection.MachineToHost, "F-theta stage L temperature");
        return nodes;

        void Add(LaserOpcNode node, string tag, OpcValueType type, NodeDirection direction, string description, NodeConstraints constraints = null)
        {
            nodes[node] = new NodeDescriptor(node, tag, type, direction, description, constraints);
        }
    }
}
}
