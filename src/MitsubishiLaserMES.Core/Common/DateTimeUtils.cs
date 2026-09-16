using System;
using System.Globalization;

namespace MitsubishiLaserMES.Core.Common
{
    public static class DateTimeUtils
    {
        public const string EapDateFormat = "yyyyMMddHHmmss";
        public const string IsoUtcFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

        public static string NowEapDate()
        {
            return DateTime.Now.ToString(EapDateFormat, CultureInfo.InvariantCulture);
        }

        public static string NowIsoUtc()
        {
            return DateTime.UtcNow.ToString(IsoUtcFormat, CultureInfo.InvariantCulture);
        }

        public static string ToEapDate(DateTime dt)
        {
            return dt.ToString(EapDateFormat, CultureInfo.InvariantCulture);
        }

        public static bool TryParseEapDate(string input, out DateTime result)
        {
            return DateTime.TryParseExact(input, EapDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }
    }
}
