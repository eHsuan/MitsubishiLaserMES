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

/// <summary>Reads every Catalog-defined node once without writing to the machine.</summary>
public sealed class CatalogReadTestService
{
    private static readonly string[] Stages = { "StageR", "StageL" };
    private static readonly string[] Beams = { "Beam0", "Beam1" };
    private readonly IOpcUaClient _client;

    public CatalogReadTestService(IOpcUaClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<IReadOnlyList<CatalogReadTestResult>> RunAsync(
        IProgress<CatalogReadTestProgress> progress = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        IReadOnlyList<NodeDescriptor> nodes = BuildReadPlan();
        var results = new List<CatalogReadTestResult>(nodes.Count);
        for (int index = 0; index < nodes.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            NodeDescriptor node = nodes[index];
            OpcReadResult read;
            try
            {
                read = await _client.ReadAsync(node, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                read = new OpcReadResult(false, null, "Bad", ex.Message);
            }

            results.Add(new CatalogReadTestResult
            {
                Time = DateTimeOffset.Now,
                Tag = node.SpecTag,
                StatusCode = read.StatusCode,
                Succeeded = read.Succeeded,
                Value = read.Value,
                Error = read.Error,
            });
            if (progress != null)
                progress.Report(new CatalogReadTestProgress(index + 1, nodes.Count, node.SpecTag));
        }
        return results;
    }

    public static IReadOnlyList<NodeDescriptor> BuildReadPlan()
    {
        var nodes = new List<NodeDescriptor>(LaserOpcNodeCatalog.All);
        AddIndexedGroup(nodes, LaserOpcNodeGroup.Mask, Enumerable.Range(1, 12));
        AddIndexedGroup(nodes, LaserOpcNodeGroup.Alarm, Enumerable.Range(0, 10));
        AddIndexedGroup(nodes, LaserOpcNodeGroup.Maintenance, LaserOpcNodeCatalog.MaintenanceItemIndexes);

        foreach (LaserOpcNodeGroup group in Enum.GetValues(typeof(LaserOpcNodeGroup)))
        {
            if (group == LaserOpcNodeGroup.Mask || group == LaserOpcNodeGroup.Alarm || group == LaserOpcNodeGroup.Maintenance)
                continue;

            NodeGroupDescriptor descriptor = LaserOpcNodeCatalog.GetGroup(group);
            string[] stages = descriptor.TagTemplate.Contains("{stage}") ? Stages : new[] { string.Empty };
            string[] beams = descriptor.TagTemplate.Contains("{beam}") ? Beams : new[] { string.Empty };
            foreach (string stage in stages)
            foreach (string beam in beams)
            foreach (string field in descriptor.Fields)
                nodes.Add(CreateGroupNode(
                    LaserOpcNodeCatalog.FormatHistoryLogTag(group, stage, beam, field),
                    descriptor));
        }

        return nodes
            .GroupBy(node => node.SpecTag, StringComparer.Ordinal)
            .Select(group => group.First())
            .OrderBy(node => node.SpecTag, StringComparer.Ordinal)
            .ToArray();
    }

    private static void AddIndexedGroup(List<NodeDescriptor> nodes, LaserOpcNodeGroup group, IEnumerable<int> indexes)
    {
        NodeGroupDescriptor descriptor = LaserOpcNodeCatalog.GetGroup(group);
        foreach (int index in indexes)
        foreach (string field in descriptor.Fields)
            nodes.Add(CreateGroupNode(LaserOpcNodeCatalog.FormatGroupTag(group, index, field), descriptor));
    }

    private static NodeDescriptor CreateGroupNode(string tag, NodeGroupDescriptor descriptor)
    {
        // The descriptor key is used only for the existing audit interface; OPC reads resolve by SpecTag.
        return new NodeDescriptor(
            LaserOpcNode.MachineType,
            tag,
            OpcValueType.String,
            descriptor.Direction,
            "Catalog read test: " + descriptor.Description);
    }
}
}
