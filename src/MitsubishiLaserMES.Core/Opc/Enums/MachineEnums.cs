using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace MitsubishiLaserOpc.Enums
{

public enum MachineOperatingMode : short
{
    Offline = 0,
    OnlineLocal = 1,
    OnlineSemiAuto = 2,
    OnlineAuto = 3,
}

public enum MachineStatus : int
{
    MachineDown = 0,
    Ready = 1,
    Running = 2,
    Idle = 3,
    Maintain = 4,
    Stop = 5,
}

public enum NodeDirection
{
    MachineToHost,
    HostToMachine,
}

public enum OpcValueType
{
    Boolean,
    Int16,
    UInt16,
    Int32,
    UInt64,
    Single,
    String,
}
}
