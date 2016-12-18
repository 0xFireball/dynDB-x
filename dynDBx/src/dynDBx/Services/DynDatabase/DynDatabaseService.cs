using dynDBx.Models;
using dynDBx.Services.Database;
using dynDBx.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace dynDBx.Services.DynDatabase
{
    public static class DynDatabaseService
    {
        public static async Task PlaceData(JObject dataBundleRoot, string path, NodeDataOvewriteMode ovewriteMode)
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
                            FlattenedJObject = JsonFlattener.FlattenJObject(dataBundleRoot)
                        });
                        trans.Commit();
                    }
                }
                var rootObjectContainer = store.FindAll().FirstOrDefault();
                var flattenedRootObject = rootObjectContainer.FlattenedJObject;
                //var rootObjectJ = JsonFlattener.UnflattenJObject(flattenedRootObject);
                var flattenedBundle = JsonFlattener.FlattenJObject(dataBundleRoot);

                using (var trans = db.BeginTrans())
                {
                    // Put in the new data
                    switch (ovewriteMode)
                    {
                        case NodeDataOvewriteMode.Update:
                            //selectedNode.Merge(dataBundleRoot);
                            flattenedRootObject.MergeInto(flattenedBundle);
                            break;

                        case NodeDataOvewriteMode.Put:
                            //if (selectedNode.Parent != null)
                            //{
                            //    selectedNode.Replace(dataBundleRoot);
                            //}
                            //else // Root node
                            //{
                            //    rootObjectToken = dataBundleRoot;
                            //}
                            break;

                        case NodeDataOvewriteMode.Push:
                            // TODO!
                            throw new NotImplementedException();
                            break;
                    }
                    // Update and store
                    rootObjectContainer.FlattenedJObject = rootObjectToken.ToString(Formatting.None);
                    store.Update(rootObjectContainer);
                    trans.Commit();
                }
                // Data was written
            });
        }

        public static async Task DeleteData(string path)
        {
            var convTokenPath = DynPathUtilities.ConvertUriPathToTokenPath(path);
            await Task.Run(() =>
            {
                var db = DatabaseAccessService.OpenOrCreateDefault();
                var store = db.GetCollection<JsonObjectStoreContainer>(DatabaseAccessService.GetDataStoreKey(0));

                var rootObjectContainer = store.FindAll().FirstOrDefault();
                using (var trans = db.BeginTrans())
                {
                    var rootObjectToken = JObject.Parse(rootObjectContainer.FlattenedJObject);
                    var removeTok = rootObjectToken.SelectToken(convTokenPath);
                    if (removeTok.Parent != null)
                    {
                        removeTok.Parent.Remove();
                    }
                    else
                    {
                    // This is a root token. Possibly add protection in the future
                    // Replace with a fresh token
                    rootObjectToken = new JObject();
                    }
                // Update and store
                rootObjectContainer.FlattenedJObject = rootObjectToken.ToString(Formatting.None);
                    store.Update(rootObjectContainer);
                    trans.Commit();
                }
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
                var rootObjectToken = JObject.Parse(rootObjectContainer.FlattenedJObject);
                return (JObject)rootObjectToken.SelectToken(convTokenPath);
            });
        }
    }
}