using System.Collections.Generic;
using System.IO;
using System.Text;
using MitsubishiLaserOpc.Models;

namespace MitsubishiLaserOpc.Services
{

/// <summary>Writes a complete browsed address-space snapshot for offline SPEC comparison.</summary>
public static class OpcAddressSpaceCsvExporter
{
    private const string Header = "BrowsePath,NodeId,ParentNodeId,NamespaceIndex,NamespaceUri,NodeClass,BrowseName,DisplayName,DataType,ValueRank,AccessLevel,UserAccessLevel,TypeDefinition,Error";

    public static void Write(string filePath, IEnumerable<OpcAddressSpaceNode> nodes)
    {
        string directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

        using (var writer = new StreamWriter(filePath, false, new UTF8Encoding(true)))
        {
            writer.WriteLine(Header);
            foreach (OpcAddressSpaceNode node in nodes)
            {
                writer.WriteLine(string.Join(",", new[]
                {
                    Escape(node.BrowsePath),
                    Escape(node.NodeId),
                    Escape(node.ParentNodeId),
                    node.NamespaceIndex.ToString(),
                    Escape(node.NamespaceUri),
                    Escape(node.NodeClass),
                    Escape(node.BrowseName),
                    Escape(node.DisplayName),
                    Escape(node.DataType),
                    Escape(node.ValueRank),
                    Escape(node.AccessLevel),
                    Escape(node.UserAccessLevel),
                    Escape(node.TypeDefinition),
                    Escape(node.Error),
                }));
            }
        }
    }

    private static string Escape(string value)
    {
        string text = value ?? string.Empty;
        return "\"" + text.Replace("\"", "\"\"") + "\"";
    }
}
}
