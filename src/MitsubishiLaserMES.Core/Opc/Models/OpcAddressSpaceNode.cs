namespace MitsubishiLaserOpc.Models
{

/// <summary>One node discovered by recursively browsing the OPC UA Objects folder.</summary>
public sealed class OpcAddressSpaceNode
{
    public string BrowsePath { get; set; }
    public string NodeId { get; set; }
    public string ParentNodeId { get; set; }
    public ushort NamespaceIndex { get; set; }
    public string NamespaceUri { get; set; }
    public string NodeClass { get; set; }
    public string BrowseName { get; set; }
    public string DisplayName { get; set; }
    public string DataType { get; set; }
    public string ValueRank { get; set; }
    public string AccessLevel { get; set; }
    public string UserAccessLevel { get; set; }
    public string TypeDefinition { get; set; }
    public string Error { get; set; }
}
}
