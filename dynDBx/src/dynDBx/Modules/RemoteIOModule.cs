using dynDBx.Services.DynDatabase;
using dynDBx.Utilities;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace dynDBx.Modules
{
    public class RemoteIOModule : NancyModule
    {
        public RemoteIOModule() : base("/io")
        {
            var pathRoutes = new[] { "/{path?}", "/{path*}" };
            foreach (var pathRoute in pathRoutes)
            {
                Put(pathRoute, HandlePutData);
                Patch(pathRoute, HandlePatchData);
                Post(pathRoute, HandlePostData);
                Delete(pathRoute, HandleDeleteData);
                Get(pathRoute, HandleGetData);
            }
        }

        private async Task<Response> HandlePutData(dynamic args)
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
        }

        private async Task<Response> HandlePatchData(dynamic args)
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
        }

        private async Task<Response> HandlePostData(dynamic args)
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
            var pushId = await DynDatabaseService.PlaceData(dataBundleJ, path, NodeDataOvewriteMode.Push);

            // Return data written
            return Response.AsJsonNet(new { name = pushId });
        }

        private async Task<Response> HandleDeleteData(dynamic args)
        {
            var path = (string)args.path ?? "";
            await DynDatabaseService.DeleteData(path);

            return Response.FromJsonString(new JObject().ToString());
        }

        private async Task<Response> HandleGetData(dynamic args)
        {
            var path = (string)args.path ?? "";
            var dataBundleJt = await DynDatabaseService.GetData(path);

            if (dataBundleJt == null)
            {
                return HttpStatusCode.NotFound;
            }

            //var dataBundle = dataBundleJt.ToObject<ExpandoObject>();
            return Response.FromJsonString(dataBundleJt.ToString());
        }
    }
}