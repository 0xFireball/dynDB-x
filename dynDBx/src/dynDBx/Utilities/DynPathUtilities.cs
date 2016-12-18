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
            // This will get rid of double slashes and other nonsense
            var joinedSegments = string.Join(".", ConvertUriPathToSegments(uriPath));
            return joinedSegments;
        }

        public static string ConvertUriPathToTokenPrefix(string uriPath)
        {
            var tokPath = ConvertUriPathToTokenPath(uriPath);
            // if tokPath isn't empty, it needs . suffix to be a valid prefix
            if (tokPath.Length > 0) tokPath += ".";
            return tokPath;
        }

        public static string AppendToTokenPrefix(string convTokenPrfx, string pushId)
        {
            return convTokenPrfx + pushId + ".";
        }
    }
}