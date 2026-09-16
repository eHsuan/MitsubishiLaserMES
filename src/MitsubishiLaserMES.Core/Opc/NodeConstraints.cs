using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace MitsubishiLaserOpc.Nodes
{

/// <summary>Optional validation metadata used by the API and test UI before a write.</summary>
public sealed class NodeConstraints
{
    public int? MaxStringLength { get; set; }
    public decimal? Minimum { get; set; }
    public decimal? Maximum { get; set; }
    public IReadOnlyCollection<long> AllowedIntegerValues { get; set; }

    public bool IsValid(object value)
    {
        string text = value as string;
        if (text != null)
        {
            return !MaxStringLength.HasValue || text.Length <= MaxStringLength.Value;
        }

        if (value is IConvertible)
        {
            try
            {
                var numericValue = Convert.ToDecimal(value, System.Globalization.CultureInfo.InvariantCulture);
                if (Minimum.HasValue && numericValue < Minimum.Value) return false;
                if (Maximum.HasValue && numericValue > Maximum.Value) return false;
                if (AllowedIntegerValues != null && !AllowedIntegerValues.Contains(Convert.ToInt64(value))) return false;
            }
            catch (FormatException) { return false; }
            catch (InvalidCastException) { return false; }
            catch (OverflowException) { return false; }
        }

        return true;
    }
}
}
