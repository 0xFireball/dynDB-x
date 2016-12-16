using Newtonsoft.Json.Linq;

namespace dynDBx.Models
{
    public class JsonObjectStoreContainer : DatabaseObject
    {
        public JObject JObject { get; set; }
    }
}