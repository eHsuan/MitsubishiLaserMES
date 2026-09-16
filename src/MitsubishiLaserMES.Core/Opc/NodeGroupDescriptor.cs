using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MitsubishiLaserOpc.Enums;

namespace MitsubishiLaserOpc.Nodes
{

public sealed class NodeGroupDescriptor
{
    public NodeGroupDescriptor(
        LaserOpcNodeGroup group,
        string tagTemplate,
        NodeDirection direction,
        IReadOnlyCollection<string> fields,
        string description)
    {
        Group = group;
        TagTemplate = tagTemplate;
        Direction = direction;
        Fields = fields;
        Description = description;
    }

    public LaserOpcNodeGroup Group { get; }
    public string TagTemplate { get; }
    public NodeDirection Direction { get; }
    public IReadOnlyCollection<string> Fields { get; }
    public string Description { get; }
}
}
