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

            var parser = new JsonParser(json, converters);
            
            return (T)parser.FromJson(typeof(T));
        }
        

        /// <summary>
        /// Serializes an object to its JSON text representation.
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <param name="indent">If true, formats output text.</param>
        /// <param name="converters">Custom type converters.</param>
        /// <returns></returns>
        public static string Serialize(object obj, bool indent = false, params IJsonConverter[] converters)
        {
            if (obj == null)
                return "null";

            var converter = converters.FirstOrDefault(c => c.GetConvertingType() == obj.GetType());

            if (converter != null)
                return Serialize(converter.Serializer(obj), indent, converters);

            if (obj is bool || obj is bool?)
                return ((bool)obj) ? "true" : "false";

            if (obj is string)
                return string.Format("\"{0}\"", obj);

            if (obj is int || obj is int?
             || obj is short || obj is short?
             || obj is long || obj is long?
             || obj is uint || obj is uint?
             || obj is ushort || obj is ushort?
             || obj is ulong || obj is ulong?
             || obj is float || obj is float?
             || obj is double || obj is double?
             || obj is decimal || obj is decimal?
             || obj is float || obj is uint?
             || obj is ushort || obj is ushort?
             || obj is ulong || obj is ulong?)
                return string.Format(CultureInfo.InvariantCulture, "{0}", obj);

            if (obj is IDictionary)
            {
                return (indent ? "{\n" : "{")
                     + string.Join(indent ? ",\n" : ",", ((IDictionary)obj)
                        .OfType<object>()
                        .Select(o => indent ? ("\t" + Serialize(o, indent, converters).Replace("\n", "\n\t")) : Serialize(o, indent, converters)))
                     + (indent ? "\n}" : "}");
            }

            if (obj is IEnumerable)
            {
                return (indent ? "[\n" : "[")
                     + string.Join(indent ? ",\n" : ",", ((IEnumerable)obj)
                        .OfType<object>()
                        .Select(o => indent ? ("\t" + Serialize(o, indent, converters).Replace("\n", "\n\t")) : Serialize(o, indent, converters)))
                     + (indent ? "\n]" : "]");
            }

            if (obj is DateTime || obj is DateTime?)
            {
                return Serialize(((DateTime)obj).ToString(CultureInfo.InvariantCulture));
            }

            if (obj is Enum)
            {
                return Serialize((int)obj);
            }


            var ot = obj.GetType();

            if (ot.IsGenericType)
            {
                var kvt = typeof(KeyValuePair<,>).MakeGenericType(ot.GenericTypeArguments);

                if (obj.GetType() == kvt)
                {
                    var k = kvt.GetProperty("Key").GetValue(obj);
                    var v = kvt.GetProperty("Value").GetValue(obj);

                    return string.Format(
                        indent ? "{0} : {1}" : "{0}:{1}",
                        Serialize(k),
                        indent ? Serialize(v, indent, converters).Replace("\n", "\n\t") : Serialize(v, indent, converters));
                }
            }

            if (obj.GetType().IsValueType)
                return null;

            return (indent ? "{\n" : "{")
                + string.Join(indent ? ",\n" : ",",
                    obj.GetType().GetFields()
                    .Select(f => string.Format(
                        indent ? "\t{0} : {1}" : "{0}:{1}",
                        Serialize(f.Name),
                        indent ? Serialize(f.GetValue(obj), indent, converters).Replace("\n", "\n\t") : Serialize(f.GetValue(obj), indent, converters))))
                + (indent ? "\n}" : "}");
        }

    }

}