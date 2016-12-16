using dynDBx.Models;
using dynDBx.Services.Database;
using dynDBx.Utilities;
using LiteDB;
using Newtonsoft.Json.Linq;
using System;
using System.Dynamic;
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
                var store = db.GetCollection<JsonObjectStoreContainer>(DatabaseAccessService.GetDataStoreKey(0));
                if (store.Count() == 0)
                {
                    using (var trans = db.BeginTrans())
                    {
                        var newRoot = new JObject();
                        store.Insert(new JsonObjectStoreContainer
                        {
                            ContainerId = Guid.NewGuid(),
                            JObject = newRoot,
                        });
                        trans.Commit();
                    }
                }
                var rootObjectContainer = store.FindOne(Query.All());
                var rootObject = (JObject)rootObjectContainer.JObject;
                using (var trans = db.BeginTrans())
                {
                    var existingObject = rootObject.SelectToken(convTokenPath);
                    if (existingObject == null)
                    {
                    }
                    trans.Commit();
                }
                // Data was written
            });
        }
    }
}