using System;
using System.Linq;

namespace IridiumIon.JsonFlat2.Internal
{
    public static class StringUtils
    {
        public static bool StartsWithAny(this string str, string[] patterns, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            return patterns.Any(p => str.StartsWith(p, comparisonType));
        }
    }
}