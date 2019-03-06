using CCShop.Common.Http;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CCShop.Common.Util
{
    public class SessionCookieUtil
    {

        public static void SetSession<T>(string key, T data)
        {
            var bytes = Serialize(data);
            HttpContext.Current.Session.Set(key, bytes);
        }

        public static void SetSessionString(string key, string data)
        {
            var bytes = Serialize(data);
            HttpContext.Current.Session.Set(key, bytes);
        }

        public static void SetSession(string key, object data)
        {
            var bytes = Serialize(data);
            HttpContext.Current.Session.Set(key, bytes);
        }

        public static T GetSession<T>(string key)
        {
            try
            {
                byte[] bytes = new byte[] { };
                HttpContext.Current.Session.TryGetValue(key, out bytes);
                return Deserialize<T>(bytes);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static string GetSessionString(string key)
        {
            byte[] bytes = new byte[] { };
            HttpContext.Current.Session.TryGetValue(key, out bytes);
            string json = System.Text.Encoding.UTF8.GetString(bytes);
            return json;
        }

        public static object GetSession(string key)
        {
            byte[] bytes = new byte[] { };
            HttpContext.Current.Session.TryGetValue(key, out bytes);
            return DeserializeObject(bytes);
        }




        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static byte[] Serialize(object obj)
        {
            if (obj == null)
                return null;

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                var data = memoryStream.ToArray();
                return data;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private static T Deserialize<T>(byte[] data)
        {

            if (data == null)
                return default(T);

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream(data))
            {
                var result = (T)binaryFormatter.Deserialize(memoryStream);
                return result;
            }

        }

        /// <summary>
        /// 把字节反序列化成相应的对象
        /// </summary>
        /// <param name="pBytes">字节流</param>
        /// <returns>object</returns>
        private static object DeserializeObject(byte[] pBytes)
        {
            object _newOjb = null;
            if (pBytes == null)
                return _newOjb;
            System.IO.MemoryStream _memory = new System.IO.MemoryStream(pBytes);
            _memory.Position = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            _newOjb = formatter.Deserialize(_memory);
            _memory.Close();
            return _newOjb;
        }


    }



}
