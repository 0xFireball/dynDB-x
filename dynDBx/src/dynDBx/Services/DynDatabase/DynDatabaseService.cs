using dynDBx.Models;
using dynDBx.Services.Database;
using dynDBx.Utilities;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace dynDBx.Services.DynDatabase
{
    public static class DynDatabaseService
    {
        public static async Task WriteData(ExpandoObject dataBundle, string path)
        {
            var convTokenPath = DynPathUtilities.ConvertUriPathToTokenPath(path);
            await Task.Run(() =>
            {
                var db = DatabaseAccessService.OpenOrCreateDefault();
                Guid baseContainerGuid;
                var store = db.GetCollection<JsonObjectStoreContainer>(DatabaseAccessService.GetDataStoreKey(0));
                if (store.Count() == 0)
                {
                    using (var trans = db.BeginTrans())
                    {
                        var newRoot = new JObject();
                        baseContainerGuid = Guid.NewGuid();
                        store.Insert(new JsonObjectStoreContainer
                        {
                            ContainerId = baseContainerGuid,
                            //JObject = newRoot,
                            JObject = JsonConvert.SerializeObject(dataBundle)
                        });
                        trans.Commit();
                    }
                }
                var rootObjectContainer = store.FindAll().FirstOrDefault();
                var rootObject = JsonConvert.DeserializeObject<ExpandoObject>(rootObjectContainer.JObject);
                using (var trans = db.BeginTrans())
                {
                //    var existingObject = rootObject.SelectToken(convTokenPath);
                //    if (existingObject == null)
                //    {
                //    }
                    trans.Commit();
                }
                // Data was written
            });
        }
    }
}