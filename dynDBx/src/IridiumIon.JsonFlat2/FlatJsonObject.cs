using IridiumIon.JsonFlat2.Internal;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace IridiumIon.JsonFlat2
{
    /// <summary>
    /// An abstraction representing a flattened JSON object
    /// </summary>
    public class FlatJsonObject
    {
        public Dictionary<string, string> Dictionary { get; }

        public FlatJsonObject() : this(new JObject())
        {
        }

        public FlatJsonObject(JObject jsonObject, string prefix = "")
        {
            Dictionary = JsonFlattener.FlattenJObject(jsonObject, prefix);
        }

        public FlatJsonObject(Dictionary<string, string> flattenedDict)
        {
            Dictionary = flattenedDict;
        }

        public JObject Unflatten()
        {
            return JsonFlattener.UnflattenJObject(Dictionary);
        }

        public static implicit operator Dictionary<string, string>(FlatJsonObject instance)
        {
            return instance.Dictionary;
        }
    }
}