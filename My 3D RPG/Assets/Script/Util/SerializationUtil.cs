using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace ProjectChan.Util
{
    /// <summary>
    /// => 데이터 직렬화를 도와주는 클래스
    /// </summary>
    public static class SerializationUtil
    {
        /// <summary>
        /// => 파라미터로 받은 Json을 지정한 T타입의 목록 형태로 역질렬화 하여 반환 하는 메서드
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<T> FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        /// <summary>
        /// => 파라미터로 받은 Json을 지정한 T타입으로 역직렬화 하여 반환 하는 메서드
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// => T 타입 데이터를 Json으로 직렬화 하여 반환
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}