using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;
using Opc.Ua.Server;
using MitsubishiLaserMES.Core.Simulator;
using MitsubishiLaserOpc.Enums;
using MitsubishiLaserOpc.Nodes;

namespace MitsubishiLaserMES.Core.Opc.Server
{
    public class LaserSimulatorNodeManager : CustomNodeManager2
    {
        private readonly ILaserMachineSimulator _simulator;
        private readonly Dictionary<string, BaseDataVariableState> _variables = new(StringComparer.Ordinal);
        public const string LaserNamespaceUri = "urn:Mitsubishi:LaserSimulator";

        public LaserSimulatorNodeManager(IServerInternal server, ApplicationConfiguration configuration, ILaserMachineSimulator simulator)
            : base(server, configuration, LaserNamespaceUri)
        {
            _simulator = simulator;
            SystemContext.NodeIdFactory = this;
        }

        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                // 1. 建立根資料夾 MitsubishiLaser
                var rootFolder = new FolderState(null)
                {
                    NodeId = new NodeId("MitsubishiLaser", NamespaceIndex),
                    BrowseName = new QualifiedName("MitsubishiLaser", NamespaceIndex),
                    DisplayName = new LocalizedText("MitsubishiLaser"),
                    TypeDefinitionId = ObjectTypeIds.FolderType
                };

                AddPredefinedNode(SystemContext, rootFolder);

                IList<IReference> references;
                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                }
                references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, rootFolder.NodeId));

                // 2. 註冊 LaserOpcNodeCatalog 中的所有節點
                foreach (var desc in LaserOpcNodeCatalog.All)
                {
                    var variable = new BaseDataVariableState(rootFolder)
                    {
                        NodeId = new NodeId(desc.SpecTag, NamespaceIndex),
                        BrowseName = new QualifiedName(desc.SpecTag, NamespaceIndex),
                        DisplayName = new LocalizedText(desc.SpecTag),
                        TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
                        DataType = MapDataType(desc.ValueType),
                        ValueRank = ValueRanks.Scalar,
                        AccessLevel = AccessLevels.CurrentReadOrWrite,
                        UserAccessLevel = AccessLevels.CurrentReadOrWrite,
                        StatusCode = StatusCodes.Good
                    };

                    object initialVal = _simulator.HandleHostRead(desc) ?? GetDefaultValue(desc.ValueType);
                    variable.Value = initialVal;

                    // 綁定寫入處理器
                    var localDesc = desc;
                    variable.OnSimpleWriteValue = (ISystemContext context, NodeState node, ref object val) =>
                    {
                        try
                        {
                            _simulator.HandleHostWrite(localDesc, val);
                            return ServiceResult.Good;
                        }
                        catch (Exception ex)
                        {
                            return new ServiceResult(StatusCodes.BadUnexpectedError, ex);
                        }
                    };

                    rootFolder.AddChild(variable);
                    AddPredefinedNode(SystemContext, variable);
                    _variables[desc.SpecTag] = variable;
                }

                // 3. 監聽引擎狀態變更，自動同步至 Address Space
                _simulator.StateChanged += OnSimulatorStateChanged;
            }
        }

        private void OnSimulatorStateChanged()
        {
            // 透過 ThreadPool 非同步執行，絕不阻塞觸發事件的執行緒
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    // 階段一：在不持有 OPC Lock 的情況下，先由模擬器讀取節點值快照 (避免與伺服器 Lock 發生死鎖)
                    var snapshots = new List<(string tag, object val)>();
                    foreach (var desc in LaserOpcNodeCatalog.All)
                    {
                        object val = _simulator.HandleHostRead(desc);
                        if (val != null)
                        {
                            snapshots.Add((desc.SpecTag, val));
                        }
                    }

                    // 階段二：獨立獲取 OPC Server Lock，批次更新節點數值 (此時已不持有模擬器 _lock)
                    lock (Lock)
                    {
                        foreach (var (tag, val) in snapshots)
                        {
                            if (_variables.TryGetValue(tag, out var variable))
                            {
                                if (!Equals(variable.Value, val))
                                {
                                    variable.Value = val;
                                    variable.ClearChangeMasks(SystemContext, false);
                                }
                            }
                        }
                    }
                }
                catch { }
            });
        }

        private static NodeId MapDataType(OpcValueType valueType)
        {
            return valueType switch
            {
                OpcValueType.Boolean => DataTypeIds.Boolean,
                OpcValueType.Int16 => DataTypeIds.Int16,
                OpcValueType.UInt16 => DataTypeIds.UInt16,
                OpcValueType.Int32 => DataTypeIds.Int32,
                OpcValueType.UInt64 => DataTypeIds.UInt64,
                OpcValueType.Single => DataTypeIds.Float,
                OpcValueType.String => DataTypeIds.String,
                _ => DataTypeIds.String
            };
        }

        private static object GetDefaultValue(OpcValueType valueType)
        {
            return valueType switch
            {
                OpcValueType.Boolean => false,
                OpcValueType.Int16 => (short)0,
                OpcValueType.UInt16 => (ushort)0,
                OpcValueType.Int32 => 0,
                OpcValueType.UInt64 => 0UL,
                OpcValueType.Single => 0.0f,
                OpcValueType.String => string.Empty,
                _ => string.Empty
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _simulator.StateChanged -= OnSimulatorStateChanged;
            }
            base.Dispose(disposing);
        }
    }
}
