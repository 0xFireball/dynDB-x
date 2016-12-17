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
            Put("/{path*}", async args =>
            {
                var path = (string)args.path;
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
                return dataBundleJ.ToString();
            });

            Patch("/{path*}", async args =>
            {
                var path = (string)args.path;
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
                return dataBundleJ.ToString();
            });

            Post("/{path*}", async args =>
            {
                var path = (string)args.path;
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
                return dataBundleJ.ToString();
            });

            Delete("/{path*}", async args =>
            {
                var path = (string)args.path;
                await DynDatabaseService.DeleteData(path);

                return Response.AsJsonNet(null);
            });

            Get("/{path*}", async args =>
            {
                var path = (string)args.path;
                var dataBundleJt = await DynDatabaseService.GetData(path);

                if (dataBundleJt == null)
                {
                    return HttpStatusCode.NotFound;
                }

                //var dataBundle = dataBundleJt.ToObject<ExpandoObject>();
                return dataBundleJt.ToString();
            });
        }
    }
}