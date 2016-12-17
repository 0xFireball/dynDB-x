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

                // Write data
                await DynDatabaseService.PutData(dataBundleJ, path);

                // Return data written
                return dataBundleJ.ToString();
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