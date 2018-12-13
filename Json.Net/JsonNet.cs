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
        public static T Deserialize<T>(Stream stream, params IJsonConverter[] converters)
        {
            return (T)new JsonParser(new StreamReader(stream), converters)
                   .FromJson(typeof(T));
        }


        /// <summary>
        /// Deserializes an object from a JSON text stream.
        /// </summary>
        /// <typeparam name="T">Deserialized object's type</typeparam>
        /// <param name="reader">Source JSON text reader</param>
        /// <param name="converters">Custom converters.</param>
        /// <returns></returns>
        public static T Deserialize<T>(TextReader reader, params IJsonConverter[] converters)
        {
            return (T)new JsonParser(reader, converters)
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
            var sw = new StringWriter();

            new JsonSerializer(sw).Serialize(obj, converters);
            
            return sw.ToString();
        }

        /// <summary>
        /// Serializes an object to its JSON text representation and writes to specified stream.
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <param name="stream">Destination stream</param>
        /// <param name="converters">Custom type converters.</param>
        /// <returns></returns>
        public static void Serialize(object obj, Stream stream, params IJsonConverter[] converters)
        {
            using (var sw = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                new JsonSerializer(sw).Serialize(obj, converters);
                sw.Flush();
            }
        }


        /// <summary>
        /// Serializes an object to its JSON text representation and writes to specified stream.
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <param name="writer">Destination text writer</param>
        /// <param name="converters">Custom type converters.</param>
        /// <returns></returns>
        public static void Serialize(object obj, TextWriter writer, params IJsonConverter[] converters)
        {
            new JsonSerializer(writer).Serialize(obj, converters);
            writer.Flush();
        }

    }

}