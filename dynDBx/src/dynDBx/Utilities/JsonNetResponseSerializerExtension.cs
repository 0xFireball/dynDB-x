using Nancy;
using Newtonsoft.Json;

namespace dynDBx.Utilities
{
    public static class JsonNetResponseSerializerExtension
    {
        public static Response AsJsonNet<T>(this IResponseFormatter formatter, T instance)
        {
            var responseData = JsonConvert.SerializeObject(instance);
            return formatter.AsText(responseData, "application/json");
        }

        public static Response AsJsonNet(this IResponseFormatter formatter, object instance)
        {
            var responseData = JsonConvert.SerializeObject(instance);
            return formatter.AsText(responseData, "application/json");
        }

        public static Response FromJsonString(this IResponseFormatter formatter, string jsonString)
        {
            return formatter.AsText(jsonString, "application/json");
        }
    }
}