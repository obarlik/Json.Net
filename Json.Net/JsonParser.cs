using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Linq;


namespace Json.Net
{
    /// <summary>
    /// JSON Parser Class
    /// </summary>
    public class JsonParser : ParserBase
    {
        IJsonConverter[] Converters;

        static Dictionary<char, char> EscapeMap = 
            new Dictionary<char, char>()
            {
                { 'b', (char)8 },
                { 't', (char)9 },
                { 'n', (char)10 },
                { 'f', (char)12 },
                { 'r', (char)13 }
            };


        public JsonParser(TextReader jsonReader, params IJsonConverter[] converters)
            : base(jsonReader)
        {
            Converters = converters;
        }
        

        object FromJsonType(object obj, Type targetType)
        {
            if (obj == null)
                return null;

            var objType = obj.GetType();

            if (targetType == objType)
                return obj;

            if (objType.IsSubclassOf(targetType))
                return obj;

            if (obj is string)
            {
                var converter = Converters.FirstOrDefault(c => c.GetConvertingType() == targetType);

                if (converter != null)
                    return converter.Deserializer((string)obj);
            }

            if (targetType.IsArray)
            {
                var et = targetType.GetElementType();
                var l = ((IList)obj).Cast<object>()
                        .Select(o => FromJsonType(o, et))
                        .ToArray();

                var t = Array.CreateInstance(et, ((IList)obj).Count);

                for (int i = 0; i < t.Length; i++)
                    t.SetValue(((IList)obj)[i], i);
                
                return t;
            }

            if (targetType.IsGenericType && targetType.GetInterface("IList") != null)
            {
                var l = (IList)obj;
                var t = (IList)Activator.CreateInstance(targetType);

                foreach (var item in l)
                    t.Add(item);

                return t;
            }

            if (targetType.IsGenericType && targetType.GetInterface("IDictionary") != null)
            {
                var d = (IDictionary)obj;
                var t = (IDictionary)Activator.CreateInstance(targetType);

                foreach (KeyValuePair<object, object> item in d)
                    t.Add(item.Key, item.Value);

                return t;
            }

            if (targetType == typeof(bool)
             || targetType == typeof(bool?))
            {
                if (obj is string)
                {
                    if ((string)obj == "true" || (string)obj == "1")
                        return true;

                    if ((string)obj == "false" || (string)obj == "0")
                        return false;
                }
                else if (obj is double)
                {
                    if ((double)obj == 1.0)
                        return true;

                    if ((double)obj == 0.0)
                        return false;
                }

                throw new FormatException("Invalid boolean value!");
            }

            if (targetType == typeof(DateTime)
             || targetType == typeof(DateTime?))
            {
                return DateTime.Parse((string)obj, CultureInfo.InvariantCulture);
            }

            if (targetType == typeof(int)
             || targetType == typeof(int?)
             || targetType == typeof(long)
             || targetType == typeof(long?)
             || targetType == typeof(short)
             || targetType == typeof(short?))
            {
                return (int)(double)obj;
            }

            if (targetType == typeof(float)
             || targetType == typeof(float?)
             || targetType == typeof(double)
             || targetType == typeof(double?))
            {
                return obj;
            }

            if (targetType == typeof(decimal)
             || targetType == typeof(decimal?))
            {
                return (decimal)(double)obj;
            }

            if (targetType.IsEnum)
            {
                if (objType == typeof(string))
                    return (int)Enum.Parse(targetType, (string)obj);

                return (int)(double)obj;
            }

            throw new FormatException("Unknown field type " + targetType.Name);
        }


        public object FromJson(Type type)
        {
            if (type == null)
                type = typeof(object);

            if (type.IsAbstract)
                throw new InvalidOperationException("Abstract types not supported!");

            object result = null;

            if (TryMatch('{'))
            {
                if (type.IsValueType)
                    throw new FormatException("Unexpected type!");

                result = Activator.CreateInstance(type);

                var nameType = result is IDictionary ?
                    type.GenericTypeArguments[0] :
                    typeof(string);

                var valueType = result is IDictionary ?
                    type.GenericTypeArguments[1] :
                    null;

                while (true)
                {
                    var name = (string)FromJson(nameType);

                    Match(":");

                    var map = SerializerMap.GetSerializerMap(type);

                    var field = valueType == null ? map.Members.FirstOrDefault(m => m.Name == name) : null;
                    var fieldType = field == null ? valueType : field.ValueType;

                    var value = FromJson(fieldType);

                    value = FromJsonType(value, fieldType);

                    if (field != null)
                    {
                        field.SetValue(result, value);
                    }
                    else
                    {
                        ((IDictionary)result).Add(name, value);
                    }

                    if (!TryMatch(','))
                        break;
                }

                Match("}");

                return result;
            }
            else if (TryMatch('['))
            {
                var elementType =
                    type.IsArray ?
                        type.GetElementType() :
                    type.IsGenericType ?
                        type.GenericTypeArguments[0] :
                        typeof(object);

                result = Activator.CreateInstance(
                            typeof(List<>).MakeGenericType(elementType));
                
                while (true)
                {
                    var item = FromJson(elementType);
                    
                    ((IList)result).Add(FromJsonType(item, elementType));

                    if (!TryMatch(','))
                        break;
                }

                Match("]");
                
                return FromJsonType(result, type);
            }
            else if (TryMatch('"'))
            {
                var text = new StringBuilder();

                while (NextChar != '"')
                {
                    if (NextChar == '\\')
                    {
                        ReadChar();

                        switch (NextChar)
                        {
                            case 'b':
                            case 't':
                            case 'n':
                            case 'f':
                            case 'r':
                                KeepChar(text, EscapeMap[NextChar]);
                                break;

                            case 'u':
                                var unicode = "";

                                ReadChar();

                                while (unicode.Length < 4 && IsHexDigit)
                                {
                                    KeepNext(ref unicode);
                                }

                                text.Append(char.ConvertFromUtf32(int.Parse("0x" + unicode)));
                                break;

                            default:
                                KeepNext(text);
                                break;
                        }
                    }
                    else
                    {
                        KeepNext(text);
                    }
                }

                Match("\"");

                return text.ToString();
            }
            else if (NextChar == 't')
            {
                Match("true");
                return true;
            }
            else if (NextChar == 'f')
            {
                Match("false");
                return false;
            }
            else if (NextChar == 'n')
            {
                Match("null");
                return null;
            }
            else if (NextChar == '-' || IsDigit)
            {
                var number = "";

                if (NextChar == '-')
                    KeepNext(ref number);

                if (NextChar == '0')
                {
                    KeepNext(ref number);
                }
                else if (IsDigit)
                {
                    while (IsDigit)
                        KeepNext(ref number);
                }
                else
                    throw new FormatException("Digit expected!");

                if (NextChar == '.')
                {
                    KeepNext(ref number);

                    while (IsDigit)
                        KeepNext(ref number);
                }

                if (NextChar == 'e' || NextChar == 'E')
                {
                    KeepChar(ref number, 'e');

                    if (NextChar == '+' || NextChar == '-')
                        KeepNext(ref number);

                    while ("0123456789".Contains(NextChar))
                        KeepNext(ref number);
                }

                return double.Parse(number, CultureInfo.InvariantCulture);
            }

            return null;
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
