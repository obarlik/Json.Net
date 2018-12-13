using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Json.Net
{
    public class JsonSerializer
    {
        TextWriter Writer;

        public JsonSerializer(TextWriter writer)
        {
            Writer = writer;
        }


        public void Serialize(object obj, params IJsonConverter[] converters)
        {
            var objectType = obj.GetType();

            if (obj == null)
            {
                Writer.Write("null");
                return;
            }

            if (obj is bool || obj is bool?)
            {
                Writer.Write(((bool)obj) ? "true" : "false");
                return;
            }

            if (obj is string)
            {
                Writer.Write(string.Format("\"{0}\"", obj));
                return;
            }

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
            {
                Writer.Write(string.Format(CultureInfo.InvariantCulture, "{0}", obj));
                return;
            }


            if (obj is IEnumerable)
            {
                Writer.Write(obj is IDictionary ? '{' : '[');

                var i = 0;

                foreach (var o in (IEnumerable)obj)
                {
                    if (++i > 1)
                        Writer.Write(',');

                    Serialize(o, converters);
                }

                Writer.Write(obj is IDictionary ? '}' : ']');
                return;
            }

            var converter = converters.FirstOrDefault(c => c.GetConvertingType() == objectType);

            if (converter != null)
            {
                Serialize(converter.Serializer(obj), converters);
                return;
            }

            if (obj is DateTime || obj is DateTime?)
            {
                Serialize(((DateTime)obj).ToString(CultureInfo.InvariantCulture), converters);
                return;
            }

            if (obj is Enum)
            {
                Serialize((int)obj, converters);
                return;
            }

            if (objectType.IsGenericType)
            {
                var kvt = typeof(KeyValuePair<,>).MakeGenericType(objectType.GenericTypeArguments);

                if (obj.GetType() == kvt)
                {
                    var k = kvt.GetProperty("Key").GetValue(obj);
                    var v = kvt.GetProperty("Value").GetValue(obj);

                    Serialize(k.ToString());
                    Writer.Write(':');
                    Serialize(v, converters);

                    return;
                }
            }

            if (!objectType.IsPrimitive)
            {
                Writer.Write('{');

                var i = 0;

                foreach(var m in SerializerMap.GetSerializerMap(objectType)
                                 .Members)
                {
                    if (++i > 1)
                        Writer.Write(',');

                    if (objectType == typeof(DictionaryEntry))
                    {
                        Serialize(((DictionaryEntry)obj).Key.ToString());
                        Writer.Write(':');
                        Serialize(((DictionaryEntry)obj).Value, converters);
                    }
                    else
                    {
                        Serialize(m.Name);
                        Writer.Write(':');
                        Serialize(m.GetValue(obj), converters);
                    }
                }

                Writer.Write('}');
                return;
            }

            throw new InvalidOperationException("Unknown object type! " + objectType.FullName);
        }
    }
}
