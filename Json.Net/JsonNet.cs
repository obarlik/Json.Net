using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;


namespace Json.Net
{
    /// <summary>
    /// Json.Net Helper Class
    /// </summary>
    public static class JsonNet
    {
        /// <summary>
        /// Deserializes an object from a JSON text.
        /// </summary>
        /// <typeparam name="T">Deserialized object's type</typeparam>
        /// <param name="json">JSON text</param>
        /// <param name="converters">Custom converters.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json, params IJsonConverter[] converters)
        {
            if ((json ?? "").Trim() == "")
                return default(T);

            return (T)new JsonParser(new StringReader(json), converters)
                   .FromJson(typeof(T));
        }


        /// <summary>
        /// Deserializes an object from a JSON text stream.
        /// </summary>
        /// <typeparam name="T">Deserialized object's type</typeparam>
        /// <param name="stream">Source JSON text stream</param>
        /// <param name="converters">Custom converters.</param>
        /// <returns></returns>
        public static T DeserializeFromStream<T>(Stream stream, params IJsonConverter[] converters)
        {
            return (T)new JsonParser(
                    new StreamReader(stream),
                    converters)
                .FromJson(typeof(T));
        }


        /// <summary>
        /// Serializes an object to its JSON text representation.
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <param name="converters">Custom type converters.</param>
        /// <returns></returns>
        public static string Serialize(object obj, params IJsonConverter[] converters)
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            new JsonSerializer(sw).Serialize(obj, converters);

            return sb.ToString();
        }


        /// <summary>
        /// Serializes an object to its JSON text representation and writes to specified stream.
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <param name="stream">Destination stream</param>
        /// <param name="converters">Custom type converters.</param>
        /// <returns></returns>
        public static void SerializeToStream(object obj, Stream stream, params IJsonConverter[] converters)
        {
            using (var sw = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                new JsonSerializer(sw).Serialize(obj, converters);
                sw.Flush();
            }
        }

    }

}