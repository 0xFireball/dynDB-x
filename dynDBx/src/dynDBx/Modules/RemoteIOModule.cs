using dynDBx.Utilities;
using Nancy;
using System.Collections.Generic;

namespace dynDBx.Modules
{
    public class RemoteIOModule : NancyModule
    {
        public RemoteIOModule() : base("/io")
        {
            Put("/{path*}", args =>
            {
                var path = (string)args.path;
                var data = (IDictionary<string, dynamic>)Request.Form;
                // Return data written
                return Response.AsJsonNet(data);
            });
        }
    }
}