using System.Collections.Generic;
using System.IO;
using System.Text;
using MitsubishiLaserOpc.Models;

namespace MitsubishiLaserOpc.Services
{

public static class CatalogReadTestCsvExporter
{
    private const string Header = "Time,Tag,StatusCode,Result,Value,Message";

    public static void Write(string filePath, IEnumerable<CatalogReadTestResult> results)
    {
        string directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        using (var writer = new StreamWriter(filePath, false, new UTF8Encoding(true)))
        {
            writer.WriteLine(Header);
            foreach (CatalogReadTestResult result in results)
            {
                writer.WriteLine(string.Join(",", new[]
                {
                    Escape(result.Time.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                    Escape(result.Tag),
                    Escape(result.StatusCode),
                    Escape(result.Succeeded ? "Success" : "Failure"),
                    Escape(result.Value == null ? string.Empty : result.Value.ToString()),
                    Escape(result.Error),
                }));
            }
        }
    }

    private static string Escape(string value)
    {
        return "\"" + (value ?? string.Empty).Replace("\"", "\"\"") + "\"";
    }
}
}
