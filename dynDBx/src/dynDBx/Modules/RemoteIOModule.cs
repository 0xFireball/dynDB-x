using dynDBx.Utilities;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;
using System.Dynamic;

namespace dynDBx.Modules
{
    public class RemoteIOModule : NancyModule
    {
        public RemoteIOModule() : base("/io")
        {
            Put("/{path*}", args =>
            {
                var path = (string)args.path;
                // Deserialize data bundle
                dynamic dataBundle = JsonConvert.DeserializeObject<ExpandoObject>(Request.Body.AsString());
                // Return data written
                return Response.AsJsonNet((object)dataBundle);
            });
        }
    }
}