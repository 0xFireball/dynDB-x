using Newtonsoft.Json.Linq;
using System;

namespace dynDBx.Models
{
    public class JsonObjectStoreContainer : DatabaseObject
    {
        public Guid ContainerId { get; set; }
        public object JObject { get; set; }
    }
}