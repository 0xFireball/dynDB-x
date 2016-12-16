using Nancy;

namespace dynDBx.Modules
{
    public class LolModule : NancyModule
    {
        public LolModule()
        {
            Get("/lol", _ => "lol, hello");
        }
    }
}