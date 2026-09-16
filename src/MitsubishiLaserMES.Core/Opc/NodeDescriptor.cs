using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MitsubishiLaserOpc.Enums;

namespace MitsubishiLaserOpc.Nodes
{

public sealed class NodeDescriptor
{
    public NodeDescriptor(
        LaserOpcNode node,
        string specTag,
        OpcValueType valueType,
        NodeDirection direction,
        string description,
        NodeConstraints constraints = null)
    {
        Node = node;
        SpecTag = specTag;
        ValueType = valueType;
        Direction = direction;
        Description = description;
        Constraints = constraints ?? new NodeConstraints();
    }

    public LaserOpcNode Node { get; }
    public string SpecTag { get; }
    public OpcValueType ValueType { get; }
    public NodeDirection Direction { get; }
    public string Description { get; }
    public NodeConstraints Constraints { get; }
    public bool IsWritableByHost => Direction == NodeDirection.HostToMachine;
    public string DisplayLabel => $"[{(IsWritableByHost ? "H→M" : "M→H")}] [{ValueType}] {Node} — {SpecTag}";
}
}
