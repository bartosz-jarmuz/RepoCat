using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RepoCat.Serialization
{
    static class Json
    {
        /// <summary>
        /// Serializes to string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Serialize(object value)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            return JsonConvert.SerializeObject(value, Formatting.Indented, settings)?.Trim(new []{'\"', ' '});
        }

        /// <summary>
        /// Deserializes the string to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object Deserialize(string value)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            try
            {
                return JsonConvert.DeserializeObject(value?.Trim(new char[]{' ', '\r', '\n'}), settings);
            }
            catch
            {
                //if we cannot deserialize the property value, just return the string as it stands.
                return value?.Trim(new char[] { ' ', '\r', '\n' });
            }
        }
    }
}
