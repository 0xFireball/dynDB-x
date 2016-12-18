using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace dynDBx.Utilities
{
    public static class JsonFlattener
    {
        /// <summary>
        /// Flatten JObject - http://stackoverflow.com/a/35838986
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static Dictionary<string, string> FlattenJObject(JObject jObj, string prefix = "")
        {
            IEnumerable<JToken> jTokens = jObj.Descendants().Where(p => p.Count() == 0);
            Dictionary<string, string> flattenedDict = jTokens.Aggregate(new Dictionary<string, string>(), (properties, jToken) =>
            {
                properties.Add(prefix + jToken.Path, jToken.ToString());
                return properties;
            });
            return flattenedDict;
        }

        public static JObject UnflattenJObject(Dictionary<string, string> flattenedObj)
        {
            return JsonUnflattenerHelper.UnflattenJObject(flattenedObj);
        }
    }
}