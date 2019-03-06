using Newtonsoft.Json;

namespace CCShop.Common.Util
{
    public class JsonUtil
    {
        public static T DeserializeObject<T>(string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static object DeserializeObject(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }

        public static string SerializeObject(object data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
