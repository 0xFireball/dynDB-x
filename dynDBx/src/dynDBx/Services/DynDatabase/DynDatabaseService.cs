using dynDBx.Models;
using dynDBx.Services.Database;
using dynDBx.Utilities;
using Newtonsoft.Json.Linq;
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
                        var newRoot = new JObject(
                            new JProperty("name", "bob")
                        );
                        store.Insert(new JsonObjectStoreContainer
                        {
                            JObject = newRoot,
                        });
                        trans.Commit();
                    }
                }
                var rootObject = store.FindOne(x => x != null).JObject;
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