using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace ProjectChan.Util
{
    public static class SerializationUtil
    {
        public static List<T> FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        public static T JsonToObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string ToJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}