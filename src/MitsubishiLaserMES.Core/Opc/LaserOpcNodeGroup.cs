using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace MitsubishiLaserOpc.Enums
{

/// <summary>Indexed or multi-dimensional OPC UA node families.</summary>
public enum LaserOpcNodeGroup
{
    Mask,
    Alarm,
    Maintenance,
    AaaGosaLog,
    PowerDataLog,
    PowerMultiLog,
    M307DataLog,
    M370DataLog,
    MaskGosaLog,
    ZAxisLog,
    LengthLog,
    SinsyukuLog,
    TableLog,
}
}
