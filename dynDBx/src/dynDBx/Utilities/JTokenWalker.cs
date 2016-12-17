using Newtonsoft.Json.Linq;
using System.Linq;

namespace dynDBx.Utilities
{
    public class JTokenWalker
    {
        private string originalQuery;
        private string[] queryFragments;
        private JObject _rootToken;

        public JTokenWalker(JObject rootObj, string queryPath)
        {
            originalQuery = queryPath;
            queryFragments = queryPath.Split('.');
            _rootToken = rootObj;
        }

        /// <summary>
        /// Walk through the token, create paths, and finally create the node
        /// </summary>
        public JObject WalkAndCreateNode()
        {
            JObject result = null;
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
            if (parentLevel == queryFragments.Length) // Nothing was found
            {
                parentLevel--; // decrement
                var newProp = new JProperty(queryFragments[0], new JObject());
                _rootToken.Add(newProp);
            }
            var foundParent = (JObject)_rootToken.SelectToken(string.Join(".", queryFragments.Take(queryFragments.Length - parentLevel)));
            JObject lastParent = foundParent;
            for (int i = parentLevel; i > 0; i--)
            {
                var nextNodeKey = queryFragments[queryFragments.Length - i];
                // Create next node
                var newObj = new JObject();
                var newProp = new JProperty(nextNodeKey, newObj);
                lastParent.Add(newProp);
                lastParent = newObj;
            }
            result = (JObject)_rootToken.SelectToken(originalQuery);
            return result;
        }
    }
}