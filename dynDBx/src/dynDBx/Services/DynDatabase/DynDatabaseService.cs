﻿using dynDBx.Models;
using dynDBx.Services.Database;
using dynDBx.Utilities;
using IridiumIon.JsonFlat2;
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
            var convTokenPrfx = DynPathUtilities.ConvertUriPathToTokenPrefix(path);
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
                            FlattenedJObject = JsonFlattener.FlattenJObject(new JObject())
                        });
                        trans.Commit();
                    }
                }
                var rootObjectContainer = store.FindAll().FirstOrDefault();
                var flattenedRootObject = rootObjectContainer.FlattenedJObject;
                //var rootObjectJ = JsonFlattener.UnflattenJObject(flattenedRootObject);
                var flattenedBundle = JsonFlattener.FlattenJObject(dataBundleRoot, convTokenPrfx);

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
                    rootObjectContainer.FlattenedJObject = flattenedRootObject;
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
                    var flattenedJObj = rootObjectContainer.FlattenedJObject;
                    FlatJsonTools.RemoveNode(convTokenPath, flattenedJObj);
                    // Update and store
                    rootObjectContainer.FlattenedJObject = flattenedJObj;
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
                var unflattenedJObj = JsonFlattener.UnflattenJObject(rootObjectContainer.FlattenedJObject);
                return (JObject)unflattenedJObj.SelectToken(convTokenPath);
            });
        }
    }
}