using Newtonsoft.Json.Linq;
using System.Linq;

namespace dynDBx.Utilities
{
    public class JTokenWalker
    {
        private string originalQuery;
        private string[] queryFragments;
        private JToken _rootToken;

        public JTokenWalker(JToken rootToken, string queryPath)
        {
            originalQuery = queryPath;
            queryFragments = queryPath.Split('.');
            _rootToken = rootToken;
        }

        /// <summary>
        /// Walk through the token, create paths, and finally create the node
        /// </summary>
        public JToken WalkAndCreateNode()
        {
            JToken result = null;
            int parentLevel = queryFragments.Length;
            for (int i = 0; i < parentLevel; i++)
            {
                var parentNodePath = string.Join(".", queryFragments.Take(queryFragments.Length - i));
                var currTokNode = _rootToken.SelectToken(parentNodePath);
                if (currTokNode != null && currTokNode is JObject)
                {
                    // Path has been found. Next tokens will be made
                    parentLevel = i;
                    break;
                }
            }
            var foundParent = (JObject)_rootToken.SelectToken(string.Join(".", queryFragments.Take(queryFragments.Length - parentLevel)));
            for (int i = parentLevel; i > 0; i--)
            {
                var nextNodeKey = queryFragments[queryFragments.Length - i];
                // Create next node
                foundParent.Add(nextNodeKey, new JObject());
            }
            result = _rootToken.SelectToken(originalQuery);
            return result;
        }
    }
}