using dynDBx.Models;
using dynDBx.Services.Database;
using dynDBx.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace dynDBx.Services.DynDatabase
{
    public static class DynDatabaseService
    {
        public static async Task PutData(JObject dataBundleRoot, string path)
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
                            JObject = JsonConvert.SerializeObject(dataBundle)
                        });
                        trans.Commit();
                    }
                }
                var rootObjectContainer = store.FindAll().FirstOrDefault();
                var rootObjectToken = JObject.Parse(rootObjectContainer.JObject);
                //var rootObject = rootObjectToken.ToObject<ExpandoObject>();
                using (var trans = db.BeginTrans())
                {
                    var selectedNode = (JObject)rootObjectToken.SelectToken(convTokenPath);
                    if (selectedNode == null)
                    {
                        // Get parent and create node with walker
                        var walker = new JTokenWalker(rootObjectToken, convTokenPath);
                        selectedNode = walker.WalkAndCreateNode();
                    }
                    // Put in the new data
                    selectedNode.Merge(dataBundleRoot);
                    rootObjectContainer.JObject = rootObjectToken.ToString(Formatting.None);
                    store.Update(rootObjectContainer);
                    trans.Commit();
                }
                // Data was written
            });
        }

        public static async Task<JObject> GetData(string path)
        {
            var convTokenPath = DynPathUtilities.ConvertUriPathToTokenPath(path);
            return await Task.Run(() =>
            {
                var db = DatabaseAccessService.OpenOrCreateDefault();
                var store = db.GetCollection<JsonObjectStoreContainer>(DatabaseAccessService.GetDataStoreKey(0));

                var rootObjectContainer = store.FindAll().FirstOrDefault();
                var rootObjectToken = JObject.Parse(rootObjectContainer.JObject);
                return (JObject)rootObjectToken.SelectToken(convTokenPath);
            });
        }
    }
}