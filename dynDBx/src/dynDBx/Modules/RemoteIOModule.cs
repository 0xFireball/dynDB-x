using dynDBx.Services.DynDatabase;
using dynDBx.Utilities;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;

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

                dynamic dataBundle = dataBundleJ.ToObject<ExpandoObject>();

                // Write data
                await DynDatabaseService.PutData(dataBundleJ, dataBundle, path);

                // Return data written
                return Response.AsJsonNet((ExpandoObject)dataBundle);
            });

            Get("/{path*}", async args =>
            {
                var path = (string)args.path;
                JToken dataBundleJt = await DynDatabaseService.GetData(path);

                var dataBundle = dataBundleJt.ToObject<ExpandoObject>();
                return Response.AsJsonNet((ExpandoObject)dataBundle);
            });
        }
    }
}