using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace dynDBx.Models
{
    public class JsonObjectStoreContainer : DatabaseObject
    {
        public Guid ContainerId { get; set; }
        public string JObject { get; set; }
    }
}