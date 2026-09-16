using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MitsubishiLaserOpc.Enums;
using MitsubishiLaserOpc.Interfaces;
using MitsubishiLaserOpc.Models;
using MitsubishiLaserOpc.Nodes;

namespace MitsubishiLaserOpc.Services
{

public sealed class NodeDiagnosticService
{
    private readonly IOpcUaClient _client;
    public NodeDiagnosticService(IOpcUaClient client) => _client = client;
    public IReadOnlyCollection<NodeDescriptor> GetAllNodes() => LaserOpcNodeCatalog.All;
    public Task<OpcReadResult> ReadAsync(LaserOpcNode node, CancellationToken cancellationToken = default(CancellationToken)) { return _client.ReadAsync(LaserOpcNodeCatalog.Get(node), cancellationToken); }
    public async Task<OperationResult> WriteAsync(LaserOpcNode node, object value, CancellationToken cancellationToken = default(CancellationToken))
    {
        var descriptor = LaserOpcNodeCatalog.Get(node);
        if (!descriptor.IsWritableByHost) return OperationResult.Failure($"{node} is read-only for Host.");
        if (!MatchesType(descriptor.ValueType, value)) return OperationResult.Failure($"{node} requires {descriptor.ValueType}.");
        if (!descriptor.Constraints.IsValid(value)) return OperationResult.Failure($"{node} value is outside its allowed range.");
        var result = await _client.WriteAsync(descriptor, value, cancellationToken).ConfigureAwait(false);
        return result.Succeeded ? OperationResult.Success() : OperationResult.Failure(result.Error ?? result.StatusCode);
    }
    private static bool MatchesType(OpcValueType type, object value)
    {
        switch (type)
        {
            case OpcValueType.Boolean: return value is bool;
            case OpcValueType.Int16: return value is short;
            case OpcValueType.UInt16: return value is ushort;
            case OpcValueType.Int32: return value is int;
            case OpcValueType.UInt64: return value is ulong;
            case OpcValueType.Single: return value is float;
            case OpcValueType.String: return value is string;
            default: return false;
        }
    }
}
}
