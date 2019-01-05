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
        public StringBuilder Builder = new StringBuilder(1024);

        protected Action<string> Write;

        public JsonSerializer()
        { }



        public JsonSerializer Initialize()
        {
            Writer = null;
            Builder.Clear();
            Write = s => Builder.Append(s);
            return this;
        }


        public JsonSerializer Initialize(TextWriter writer)
        {
            Writer = writer;
            Write = s => Writer.Write(s);
            return this;
        }


        [ThreadStatic]
        static JsonSerializer _Instance;

        public static JsonSerializer Instance
        {
            get
            {
                return _Instance ??
                      (_Instance = new JsonSerializer());
            }
        }

        
        static Dictionary<Type, Func<object, string>> SerializerCache = 
            new Dictionary<Type, Func<object, string>>();


        public void Serialize(object obj, params IJsonConverter[] converters)
        {
            if (obj == null)
            {
                Write("null");
                return;
            }

            var objectType = obj.GetType();

            var converter = converters.FirstOrDefault(c => c.GetConvertingType() == objectType);

            if (converter != null)
            {
                Serialize(converter.Serializer(obj), converters);
                return;
            }

            Func<object, string> cnv;

            if (!SerializerCache.TryGetValue(objectType, out cnv))
            {
                var strConverter = new Func<object, string>(
                    s => 
                    {
                        var t = (string)s;
                        var r = "\"";

                        for (var i = 0;i<t.Length;i++)
                        {
                            var c = t[i];

                            r += c == '\r' ? "\\r" :
                                 c == '\n' ? "\\n" :
                                 c == '\t' ? "\\t" :
                                 c == '"' ? "\\\"" :
                                 c == '\\' ? "\\\\" :
                                 c == '/' ? "\\/" :
                                 c == '\b' ? "\\b" :
                                 c == '\f' ? "\\f" :
                                 c.ToString();
                        }

                        r += "\"";

                        return r;
                    });

                if (obj is string)
                {
                    cnv = strConverter;
                }
                else if (obj is IEnumerable)
                {
                    Write(obj is IDictionary ? "{" : "[");

                    var first = true;

                    foreach (var o in (IEnumerable)obj)
                    {
                        if (first)
                            first = false;
                        else
                            Write(",");

                        Serialize(o, converters);
                    }

                    Write(obj is IDictionary ? "}" : "]");
                    return;
                }
                else if (obj is bool || obj is bool?)
                {
                    cnv = b => (bool)b ? "true" : "false";
                }
                else if (obj is int || obj is int?
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
                    cnv = n => string.Format(CultureInfo.InvariantCulture, "{0}", n);
                }
                else if (obj is DateTime || obj is DateTime?)
                {
                    cnv = d => strConverter(((DateTime)d).ToString(CultureInfo.InvariantCulture));
                }
                else if (obj is Enum)
                {
                    cnv = e => ((int)e).ToString(CultureInfo.InvariantCulture);
                }
                else if (objectType.IsGenericType)
                {
                    var kvt = typeof(KeyValuePair<,>).MakeGenericType(objectType.GenericTypeArguments);

                    if (obj.GetType() != kvt)
                        throw new InvalidDataException(
                            "Unexpected key type! '" + objectType.GenericTypeArguments[0].Name + "'");

                    var k = kvt.GetProperty("Key").GetValue(obj).ToString();
                    var v = kvt.GetProperty("Value").GetValue(obj);

                    Write(strConverter(k));
                    Write(":");
                    Serialize(v, converters);
                    return;
                }
                else if (!objectType.IsPrimitive)
                {
                    Write("{");

                    var first = true;

                    foreach (var m in SerializerMap.GetSerializerMap(objectType)
                                      .Members)
                    {
                        if (first)
                            first = false;
                        else
                            Write(",");

                        if (objectType == typeof(DictionaryEntry))
                        {
                            var d = (DictionaryEntry)obj;
                            Write(strConverter(d.Key.ToString()));
                            Write(":");
                            Serialize(d.Value, converters);
                        }
                        else
                        {
                            Write(strConverter(m.Name));
                            Write(":");
                            Serialize(m.GetValue(obj), converters);
                        }
                    }

                    Write("}");
                    return;
                }

                SerializerCache[objectType] =
                    cnv ??
                    throw new InvalidOperationException(
                        "Unknown object type! " + objectType.FullName); ;
            }

            Write(cnv(obj));
        }
    }
}
