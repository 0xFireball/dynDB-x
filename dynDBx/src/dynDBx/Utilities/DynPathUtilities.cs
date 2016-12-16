namespace dynDBx.Utilities
{
    public static class DynPathUtilities
    {
        public static string[] ConvertUriPathToSegments(string uriPath)
        {
            return uriPath.Split('/');
        }

        public static string ConvertUriPathToTokenPath(string uriPath)
        {
            return uriPath.Replace("/", ".");
        }
    }
}