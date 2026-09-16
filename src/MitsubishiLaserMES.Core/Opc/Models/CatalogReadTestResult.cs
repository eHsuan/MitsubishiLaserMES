using System;

namespace MitsubishiLaserOpc.Models
{

/// <summary>One read-only verification result for a Catalog node.</summary>
public sealed class CatalogReadTestResult
{
    public DateTimeOffset Time { get; set; }
    public string Tag { get; set; }
    public string StatusCode { get; set; }
    public bool Succeeded { get; set; }
    public object Value { get; set; }
    public string Error { get; set; }
}

public sealed class CatalogReadTestProgress
{
    public CatalogReadTestProgress(int completed, int total, string tag)
    {
        Completed = completed;
        Total = total;
        Tag = tag;
    }

    public int Completed { get; private set; }
    public int Total { get; private set; }
    public string Tag { get; private set; }
}
}
