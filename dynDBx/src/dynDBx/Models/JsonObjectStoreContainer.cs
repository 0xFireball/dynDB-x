using System;
using System.Collections.Generic;

namespace dynDBx.Models
{
    public class JsonObjectStoreContainer : DatabaseObject
    {
        public Guid ContainerId { get; set; }
        public Dictionary<string, string> FlattenedJObject { get; set; }
    }
}