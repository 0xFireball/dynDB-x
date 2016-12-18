using dynDBx.Models;
using dynDBx.Services.Database;
using dynDBx.Utilities;
using IridiumIon.JsonFlat2;
using IridiumIon.JsonFlat2.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace dynDBx.Services.DynDatabase
{
    public static class DynDatabaseService
    {
        public static async Task<string> PlaceData(JObject dataBundleRoot, string path, NodeDataOvewriteMode ovewriteMode)
        {
            string result = null; // Optionally used to return a result
            var convTokenPath = FlatJsonPath.ConvertUriPathToTokenPath(path);
            var convTokenPrfx = FlatJsonPath.ConvertUriPathToTokenPrefix(path);
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
                            FlattenedJObject = new FlatJsonObject()
                        });
                        trans.Commit();
                    }
                }
                var rootObjectContainer = store.FindAll().FirstOrDefault();
                var flattenedRootObject = new FlatJsonObject(rootObjectContainer.FlattenedJObject);
                //var rootObjectJ = JsonFlattener.UnflattenJObject(flattenedRootObject);

                using (var trans = db.BeginTrans())
                {
                    // Put in the new data
                    switch (ovewriteMode)
                    {
                        case NodeDataOvewriteMode.Update:
                            {
                                // Flatten input bundle
                                var flattenedBundle = new FlatJsonObject(dataBundleRoot, convTokenPrfx);
                                flattenedRootObject.Merge(flattenedBundle);
                            }

                            break;

                        case NodeDataOvewriteMode.Put:
                            {
                                // Flatten input bundle
                                var flattenedBundle = new FlatJsonObject(dataBundleRoot, convTokenPrfx);
                                // Remove existing data
                                flattenedRootObject.RemoveNode(convTokenPath);
                                // Add new data
                                flattenedRootObject.Merge(flattenedBundle);
                            }
                            break;

                        case NodeDataOvewriteMode.Push:
                            {
                                // Use the Firebase Push ID algorithm
                                var pushId = PushIdGenerator.GeneratePushId();
                                // Create flattened bundle with pushId added to prefix
                                convTokenPrfx = FlatJsonPath.AppendToTokenPrefix(convTokenPrfx, pushId);
                                // Flatten input bundle
                                var flattenedBundle = new FlatJsonObject(dataBundleRoot, convTokenPrfx);
                                flattenedRootObject.Merge(flattenedBundle);
                                result = pushId;
                            }
                            break;
                    }
                    // Update and store
                    rootObjectContainer.FlattenedJObject = flattenedRootObject;
                    store.Update(rootObjectContainer);
                    trans.Commit();
                }
                // Data was written
            });
            return result;
        }

        public static async Task DeleteData(string path)
        {
            var convTokenPath = FlatJsonPath.ConvertUriPathToTokenPath(path);
            await Task.Run(() =>
            {
                var db = DatabaseAccessService.OpenOrCreateDefault();
                var store = db.GetCollection<JsonObjectStoreContainer>(DatabaseAccessService.GetDataStoreKey(0));

                var rootObjectContainer = store.FindAll().FirstOrDefault();
                using (var trans = db.BeginTrans())
                {
                    var flattenedJObj = new FlatJsonObject(rootObjectContainer.FlattenedJObject);
                    flattenedJObj.RemoveNode(convTokenPath);
                    // Update and store
                    rootObjectContainer.FlattenedJObject = flattenedJObj;
                    store.Update(rootObjectContainer);
                    trans.Commit();
                }
            });
        }

        public static async Task<JToken> GetData(string path)
        {
            var convTokenPath = FlatJsonPath.ConvertUriPathToTokenPath(path);
            return await Task.Run(() =>
            {
                var db = DatabaseAccessService.OpenOrCreateDefault();
                var store = db.GetCollection<JsonObjectStoreContainer>(DatabaseAccessService.GetDataStoreKey(0));

                var rootObjectContainer = store.FindAll().FirstOrDefault();
                var unflattenedJObj = new FlatJsonObject(rootObjectContainer.FlattenedJObject).Unflatten();
                return unflattenedJObj.SelectToken(convTokenPath);
            });
        }
    }
}