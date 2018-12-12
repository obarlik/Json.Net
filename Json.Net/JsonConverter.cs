using System;
using System.Collections.Generic;
using System.Text;

namespace Json.Net
{
    /// <summary>
    /// Custom type converter for serializer and deserializer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonConverter<T> : IJsonConverter
    {
        public JsonConverter(
            Func<T, string> serializer,
            Func<string, T> deserializer)
        {
            Serializer = serializer;
            Deserializer = deserializer;
        }

        public Func<T, string> Serializer;
        public Func<string, T> Deserializer;

        Type IJsonConverter.GetConvertingType()
        {
            return typeof(T);
        }

        string IJsonConverter.Serializer(object obj)
        {
            return Serializer((T)obj);
        }

        object IJsonConverter.Deserializer(string txt)
        {
            return Deserializer(txt);
        }
    }
}
