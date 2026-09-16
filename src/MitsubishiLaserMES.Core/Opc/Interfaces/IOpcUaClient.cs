using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MitsubishiLaserOpc.Nodes;

namespace MitsubishiLaserOpc.Interfaces
{
    public interface IOpcUaClient
    {
        Task<OpcReadResult> ReadAsync(NodeDescriptor node, CancellationToken cancellationToken = default(CancellationToken));
        Task<OpcWriteResult> WriteAsync(NodeDescriptor node, object value, CancellationToken cancellationToken = default(CancellationToken));
        IDisposable Subscribe(IReadOnlyCollection<NodeDescriptor> nodes, Action<OpcSubscriptionUpdate> onUpdate);
    }

    public sealed class OpcSubscriptionUpdate
    {
        public OpcSubscriptionUpdate(NodeDescriptor node, object value, string statusCode, DateTimeOffset timestamp)
        {
            Node = node;
            Value = value;
            StatusCode = statusCode;
            Timestamp = timestamp;
        }

        public NodeDescriptor Node { get; private set; }
        public object Value { get; private set; }
        public string StatusCode { get; private set; }
        public DateTimeOffset Timestamp { get; private set; }
    }

    public sealed class OpcReadResult
    {
        public OpcReadResult(bool succeeded, object value, string statusCode, string error = null)
        {
            Succeeded = succeeded;
            Value = value;
            StatusCode = statusCode;
            Error = error;
        }

        public bool Succeeded { get; private set; }
        public object Value { get; private set; }
        public string StatusCode { get; private set; }
        public string Error { get; private set; }
    }

    public sealed class OpcWriteResult
    {
        public OpcWriteResult(bool succeeded, string statusCode, string error = null)
        {
            Succeeded = succeeded;
            StatusCode = statusCode;
            Error = error;
        }

        public bool Succeeded { get; private set; }
        public string StatusCode { get; private set; }
        public string Error { get; private set; }
    }
}
