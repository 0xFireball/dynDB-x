using System.Text.RegularExpressions;

namespace dynDBx.Utilities
{
    public class WildcardMatcher
    {
        public static bool IsMatch(string test, string pattern)
        {
            var regexPattern = ToRegex(pattern);
            var regex = new Regex(regexPattern);
            return regex.IsMatch(test);
        }

        public static string ToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern)
                              .Replace(@"\*", ".*")
                              .Replace(@"\?", ".")
                       + "$";
        }
    }
}