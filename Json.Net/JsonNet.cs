using System.IO;
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
        /// <param name="options">Serialization options.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json, SerializationOptions options = null)
        {
            return (T)JsonParser.Instance.Initialize(json, options)
                   .FromJson(typeof(T));
        }


        /// <summary>
        /// Deserializes an object from a JSON text stream.
        /// </summary>
        /// <typeparam name="T">Deserialized object's type</typeparam>
        /// <param name="stream">Source JSON text stream</param>
        /// <param name="options">Serialization options.</param>
        /// <returns></returns>
        public static T Deserialize<T>(Stream stream, SerializationOptions options = null)
        {
            return (T)JsonParser.Instance.Initialize(new StreamReader(stream), options)
                   .FromJson(typeof(T));
        }


        /// <summary>
        /// Deserializes an object from a JSON text reader.
        /// </summary>
        /// <typeparam name="T">Deserialized object's type</typeparam>
        /// <param name="reader">Source JSON text reader</param>
        /// <param name="options">Serialization options.</param>
        /// <returns></returns>
        public static T Deserialize<T>(TextReader reader, SerializationOptions options = null)
        {
            return (T)JsonParser.Instance.Initialize(reader, options)
                   .FromJson(typeof(T));
        }


        /// <summary>
        /// Serializes an object to its JSON text representation.
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <param name="options">Serialization options.</param>
        /// <returns></returns>
        public static string Serialize(object obj, SerializationOptions options = null)
        {
            var serializer = JsonSerializer.Instance.Initialize();

            serializer.Serialize(obj, options);

            return serializer.Builder.ToString();
        }


        /// <summary>
        /// Serializes an object to its JSON text representation and writes to specified stream.
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <param name="stream">Destination stream</param>
        /// <param name="options">Serialization options.</param>
        /// <returns></returns>
        public static void Serialize(object obj, Stream stream, SerializationOptions options = null)
        {
            using (var sw = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                Serialize(obj, sw, options);
            }
        }


        /// <summary>
        /// Serializes an object to its JSON text representation and writes to a textwriter.
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <param name="writer">Destination text writer</param>
        /// <param name="options">Serialization options.</param>
        /// <returns></returns>
        public static void Serialize(object obj, TextWriter writer, SerializationOptions options = null)
        {
            JsonSerializer.Instance.Initialize(writer)
            .Serialize(obj, options);

            writer.Flush();
        }


        /// <summary>
        /// Deserializes an object from a JSON text.
        /// </summary>
        /// <typeparam name="T">Deserialized object's type</typeparam>
        /// <param name="json">JSON text</param>
        /// <param name="converters">Custom converters.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json, params IJsonConverter[] converters)
        {
            return Deserialize<T>(json, new SerializationOptions { Converters = converters });
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
            return Deserialize<T>(new StreamReader(stream), new SerializationOptions { Converters = converters });
        }


        /// <summary>
        /// Deserializes an object from a JSON text reader.
        /// </summary>
        /// <typeparam name="T">Deserialized object's type</typeparam>
        /// <param name="reader">Source JSON text reader</param>
        /// <param name="converters">Custom converters.</param>
        /// <returns></returns>
        public static T Deserialize<T>(TextReader reader, params IJsonConverter[] converters)
        {
            return Deserialize<T>(reader, new SerializationOptions { Converters = converters });
        }


        /// <summary>
        /// Serializes an object to its JSON text representation.
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <param name="converters">Custom type converters.</param>
        /// <returns></returns>
        public static string Serialize(object obj, params IJsonConverter[] converters)
        {
            return Serialize(obj, new SerializationOptions { Converters = converters });
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
            Serialize(obj, stream, new SerializationOptions { Converters = converters });
        }


        /// <summary>
        /// Serializes an object to its JSON text representation and writes to a textwriter.
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <param name="writer">Destination text writer</param>
        /// <param name="converters">Custom type converters.</param>
        /// <returns></returns>
        public static void Serialize(object obj, TextWriter writer, params IJsonConverter[] converters)
        {
            Serialize(obj, new SerializationOptions { Converters = converters });
        }


        /// <summary>
        /// Deserializes an object from a JSON text.
        /// </summary>
        /// <typeparam name="T">Deserialized object's type</typeparam>
        /// <param name="json">JSON text</param>
        /// <param name="propertyNameTransform">Property name tranform</param>
        /// <param name="converters">Custom converters.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json, IPropertyNameTransform propertyNameTransform, params IJsonConverter[] converters)
        {
            return Deserialize<T>(json, new SerializationOptions { PropertyNameTransform = propertyNameTransform, Converters = converters });
        }


        /// <summary>
        /// Deserializes an object from a JSON text stream.
        /// </summary>
        /// <typeparam name="T">Deserialized object's type</typeparam>
        /// <param name="stream">Source JSON text stream</param>
        /// <param name="propertyNameTransform">Property name tranform</param>
        /// <param name="converters">Custom converters.</param>
        /// <returns></returns>
        public static T Deserialize<T>(Stream stream, IPropertyNameTransform propertyNameTransform, params IJsonConverter[] converters)
        {
            return Deserialize<T>(new StreamReader(stream), new SerializationOptions { PropertyNameTransform = propertyNameTransform, Converters = converters });
        }


        /// <summary>
        /// Deserializes an object from a JSON text reader.
        /// </summary>
        /// <typeparam name="T">Deserialized object's type</typeparam>
        /// <param name="reader">Source JSON text reader</param>
        /// <param name="propertyNameTransform">Property name tranform</param>
        /// <param name="converters">Custom converters.</param>
        /// <returns></returns>
        public static T Deserialize<T>(TextReader reader, IPropertyNameTransform propertyNameTransform, params IJsonConverter[] converters)
        {
            return Deserialize<T>(reader, new SerializationOptions { PropertyNameTransform = propertyNameTransform, Converters = converters });
        }


        /// <summary>
        /// Serializes an object to its JSON text representation.
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <param name="propertyNameTransform">Property name tranform</param>
        /// <param name="converters">Custom type converters.</param>
        /// <returns></returns>
        public static string Serialize(object obj, IPropertyNameTransform propertyNameTransform, params IJsonConverter[] converters)
        {
            return Serialize(obj, new SerializationOptions { PropertyNameTransform = propertyNameTransform, Converters = converters });
        }


        /// <summary>
        /// Serializes an object to its JSON text representation and writes to specified stream.
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <param name="stream">Destination stream</param>
        /// <param name="propertyNameTransform">Property name tranform</param>
        /// <param name="converters">Custom type converters.</param>
        /// <returns></returns>
        public static void Serialize(object obj, Stream stream, IPropertyNameTransform propertyNameTransform, params IJsonConverter[] converters)
        {
            Serialize(obj, stream, new SerializationOptions { PropertyNameTransform = propertyNameTransform, Converters = converters });
        }


        /// <summary>
        /// Serializes an object to its JSON text representation and writes to a textwriter.
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <param name="writer">Destination text writer</param>
        /// <param name="propertyNameTransform">Property name tranform</param>
        /// <param name="converters">Custom type converters.</param>
        /// <returns></returns>
        public static void Serialize(object obj, TextWriter writer, IPropertyNameTransform propertyNameTransform, params IJsonConverter[] converters)
        {
            Serialize(obj, new SerializationOptions { PropertyNameTransform = propertyNameTransform, Converters = converters });
        }

    }

}