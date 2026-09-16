using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace MitsubishiLaserOpc.Models
{

/// <summary>Runtime connection configuration. Keep credentials outside source control.</summary>
public sealed class MitsubishiOpcUaOptions
{
    public string EndpointUrl { get; set; } = string.Empty;
    public string ApplicationName { get; set; } = "MitsubishiLaserOpcClient";
    public uint SessionTimeoutMs { get; set; } = 60000;
    public bool UseSecurity { get; set; } = false;
    public bool AutoAcceptUntrustedCertificates { get; set; }
    /// <summary>
    /// Reject SHA-1 signed Server certificates. Keep true for secure production endpoints.
    /// Legacy DeviceXPlorer servers may require false when SecurityPolicy=None is used.
    /// </summary>
    public bool RejectSha1SignedCertificates { get; set; } = true;
    /// <summary>
    /// Minimum accepted RSA key size for the Server certificate. Keep 2048 for secure production endpoints.
    /// Legacy DeviceXPlorer servers using SecurityPolicy=None may require 1024.
    /// </summary>
    public ushort MinimumCertificateKeySize { get; set; } = 2048;
    public ushort DefaultNamespaceIndex { get; set; } = 2;
    public string UserName { get; set; }
    public string Password { get; set; }

    /// <summary>Directory for real-machine OPC UA audit CSV files. Files are retained indefinitely.</summary>
    public string AuditLogDirectory { get; set; }

    /// <summary>Maps the SPEC Tag (for example MACHINE.Status.Opc.Mode) to an actual OPC UA NodeId string.</summary>
    public IReadOnlyDictionary<string, string> NodeIdsBySpecTag { get; set; } =
        new Dictionary<string, string>(StringComparer.Ordinal);
}
}
