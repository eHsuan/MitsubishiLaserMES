using System;
using System.Collections.Generic;
using Opc.Ua;
using Opc.Ua.Server;
using MitsubishiLaserMES.Core.Simulator;

namespace MitsubishiLaserMES.Core.Opc.Server
{
    public class LaserSimulatorServer : StandardServer
    {
        private readonly ILaserMachineSimulator _simulator;

        public LaserSimulatorServer(ILaserMachineSimulator simulator)
        {
            _simulator = simulator ?? LaserMachineSimulatorEngine.SharedInstance;
        }

        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            var nodeManagers = new List<INodeManager>
            {
                new LaserSimulatorNodeManager(server, configuration, _simulator)
            };
            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }

        protected override ServerProperties LoadServerProperties()
        {
            return new ServerProperties
            {
                ManufacturerName = "Mitsubishi",
                ProductName = "Mitsubishi Laser Machine Simulator",
                ProductUri = "urn:Mitsubishi:LaserSimulator",
                SoftwareVersion = Utils.GetAssemblySoftwareVersion(),
                BuildNumber = Utils.GetAssemblyBuildNumber(),
                BuildDate = DateTime.UtcNow
            };
        }
    }
}
