using System;

namespace MitsubishiLaserOpc.Models
{
    /// <summary>
    /// One OPC UA communication event written to the production audit CSV file.
    /// </summary>
    public sealed class OpcAuditLogEntry
    {
        public OpcAuditLogEntry(
            DateTime timestamp,
            string direction,
            string node,
            string tag,
            object requestValue,
            object responseValue,
            string statusCode,
            bool succeeded,
            string message)
        {
            Timestamp = timestamp;
            Direction = direction;
            Node = node;
            Tag = tag;
            RequestValue = requestValue;
            ResponseValue = responseValue;
            StatusCode = statusCode;
            Succeeded = succeeded;
            Message = message;
        }

        public DateTime Timestamp { get; private set; }
        public string Direction { get; private set; }
        public string Node { get; private set; }
        public string Tag { get; private set; }
        public object RequestValue { get; private set; }
        public object ResponseValue { get; private set; }
        public string StatusCode { get; private set; }
        public bool Succeeded { get; private set; }
        public string Message { get; private set; }
    }
}
