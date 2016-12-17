using dynDBx.Services.DynDatabase;
using dynDBx.Utilities;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dynDBx.Modules
{
    public class RemoteIOModule : NancyModule
    {
        public RemoteIOModule() : base("/io")
        {
            Put(@"^(?<path>[\w\/]+)$", async args =>
            {
                var path = (string)args.path ?? "";
                // Deserialize data bundle
                JObject dataBundleJ;
                try
                {
                    dataBundleJ = JObject.Parse(Request.Body.AsString());
                }
                catch (JsonSerializationException)
                {
                    return HttpStatusCode.BadRequest;
                }

                // Write data
                await DynDatabaseService.PlaceData(dataBundleJ, path, NodeDataOvewriteMode.Put);

                // Return data written
                return Response.FromJsonString(dataBundleJ.ToString());
            });

            Patch(@"^(?<path>[\w\/]+)$", async args =>
            {
                var path = (string)args.path ?? "";
                // Deserialize data bundle
                JObject dataBundleJ;
                try
                {
                    dataBundleJ = JObject.Parse(Request.Body.AsString());
                }
                catch (JsonSerializationException)
                {
                    return HttpStatusCode.BadRequest;
                }

                // Write data
                await DynDatabaseService.PlaceData(dataBundleJ, path, NodeDataOvewriteMode.Update);

                // Return data written
                return Response.FromJsonString(dataBundleJ.ToString());
            });

            Post(@"^(?<path>[\w\/]+)$", async args =>
            {
                var path = (string)args.path ?? "";
                // Deserialize data bundle
                JObject dataBundleJ;
                try
                {
                    dataBundleJ = JObject.Parse(Request.Body.AsString());
                }
                catch (JsonSerializationException)
                {
                    return HttpStatusCode.BadRequest;
                }

                // Write data
                await DynDatabaseService.PlaceData(dataBundleJ, path, NodeDataOvewriteMode.Push);

                // Return data written
                return Response.FromJsonString(dataBundleJ.ToString());
            });

            Delete(@"^(?<path>[\w\/]+)$", async args =>
            {
                var path = (string)args.path ?? "";
                await DynDatabaseService.DeleteData(path);

                return Response.FromJsonString(new JObject().ToString());
            });

            Get(@"^(?<path>[\w\/]+)$", async args =>
            {
                var path = (string)args.path ?? "";
                var dataBundleJt = await DynDatabaseService.GetData(path);

                if (dataBundleJt == null)
                {
                    return HttpStatusCode.NotFound;
                }

                //var dataBundle = dataBundleJt.ToObject<ExpandoObject>();
                return Response.FromJsonString(dataBundleJt.ToString());
            });
        }
    }
}