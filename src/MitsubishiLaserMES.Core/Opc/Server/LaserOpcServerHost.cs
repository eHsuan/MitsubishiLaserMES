using System;
using System.IO;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Configuration;
using MitsubishiLaserMES.Core.Simulator;

namespace MitsubishiLaserMES.Core.Opc.Server
{
    public class LaserOpcServerHost : IDisposable
    {
        private LaserSimulatorServer _server;
        private ApplicationConfiguration _config;

        public bool IsRunning => _server != null && _server.CurrentState == ServerState.Running;
        public int Port { get; private set; } = 4840;

        public async Task StartAsync(ILaserMachineSimulator simulator, int port = 4840)
        {
            if (IsRunning) return;
            Port = port;

            var pkiPath = Path.Combine(AppContext.BaseDirectory, "pki_server");
            Directory.CreateDirectory(Path.Combine(pkiPath, "own"));
            Directory.CreateDirectory(Path.Combine(pkiPath, "trusted"));
            Directory.CreateDirectory(Path.Combine(pkiPath, "issuers"));
            Directory.CreateDirectory(Path.Combine(pkiPath, "rejected"));

            _config = new ApplicationConfiguration
            {
                ApplicationName = "MitsubishiLaserOpcServer",
                ApplicationUri = $"urn:{Utils.GetHostName()}:MitsubishiLaserOpcServer",
                ApplicationType = ApplicationType.Server,
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                ServerConfiguration = new ServerConfiguration
                {
                    BaseAddresses = new StringCollection { $"opc.tcp://0.0.0.0:{port}" },
                    SecurityPolicies = new ServerSecurityPolicyCollection
                    {
                        new ServerSecurityPolicy
                        {
                            SecurityMode = MessageSecurityMode.None,
                            SecurityPolicyUri = SecurityPolicies.None
                        }
                    },
                    UserTokenPolicies = new UserTokenPolicyCollection
                    {
                        new UserTokenPolicy(UserTokenType.Anonymous)
                    }
                },
                SecurityConfiguration = new SecurityConfiguration
                {
                    AutoAcceptUntrustedCertificates = true,
                    RejectSHA1SignedCertificates = false,
                    MinimumCertificateKeySize = 1024,
                    ApplicationCertificate = new CertificateIdentifier
                    {
                        StoreType = CertificateStoreType.Directory,
                        StorePath = Path.Combine(pkiPath, "own"),
                        SubjectName = "CN=MitsubishiLaserOpcServer"
                    },
                    TrustedPeerCertificates = new CertificateTrustList
                    {
                        StoreType = CertificateStoreType.Directory,
                        StorePath = Path.Combine(pkiPath, "trusted")
                    },
                    TrustedIssuerCertificates = new CertificateTrustList
                    {
                        StoreType = CertificateStoreType.Directory,
                        StorePath = Path.Combine(pkiPath, "issuers")
                    },
                    RejectedCertificateStore = new CertificateTrustList
                    {
                        StoreType = CertificateStoreType.Directory,
                        StorePath = Path.Combine(pkiPath, "rejected")
                    }
                }
            };

            await _config.ValidateAsync(ApplicationType.Server).ConfigureAwait(false);

            _server = new LaserSimulatorServer(simulator);
            await _server.StartAsync(_config).ConfigureAwait(false);
        }

        public void Stop()
        {
            if (_server != null)
            {
                try
                {
                    _server.StopAsync().GetAwaiter().GetResult();
                }
                catch { }
                _server = null;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
