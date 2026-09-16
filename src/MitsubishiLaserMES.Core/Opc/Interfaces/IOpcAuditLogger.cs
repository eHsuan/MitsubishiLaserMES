using MitsubishiLaserOpc.Models;

namespace MitsubishiLaserOpc.Interfaces
{
    /// <summary>
    /// Writes a production OPC UA audit event. Implementations must not throw to communication code.
    /// </summary>
    public interface IOpcAuditLogger
    {
        string CurrentLogFilePath { get; }
        void Write(OpcAuditLogEntry entry);
    }
}
