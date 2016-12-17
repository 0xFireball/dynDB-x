using Newtonsoft.Json.Linq;
using System;

namespace dynDBx.Models
{
    public class JsonObjectStoreContainer : DatabaseObject
    {
        public Guid ContainerId { get; set; }
        public JObject JObject { get; set; }
    }
}