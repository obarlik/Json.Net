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
    public class JsonParser
    {
        StreamReader str;
        string text;
        char next;
        IJsonConverter[] Converters;

        public JsonParser(string json, params IJsonConverter[] converters)
        {
            str =
                new StreamReader(
                  new MemoryStream(
                    Encoding.Default.GetBytes(json ?? "")));

            text = "";
            next = (char)str.Peek();

            Converters = converters;
        }




        char ReadChar()
        {
            if (!str.EndOfStream)
                return (char)str.Read();

            throw new FormatException("Unexpected end of string!");
        }

        void MoveChar()
        {
            ReadChar();

            if (!str.EndOfStream)
                next = (char)str.Peek();
            else
                next = char.MinValue;
        }

        void SkipWhite()
        {
            while (" \t\r\n".Contains(next))
                MoveChar();
        }

        void AppendChar()
        {
            text += next;
            MoveChar();
        }

        bool TryMatch(char c)
        {
            SkipWhite();

            if (next == c)
            {
                MoveChar();
                return true;
            }

            return false;
        }

        void Match(char c)
        {
            SkipWhite();

            if (next == c)
                MoveChar();
            else
                throw new FormatException(c + " expected!");
        }


        object FromJsonType(object obj, Type targetType)
        {
            if (obj == null)
                return null;

            if (targetType == obj.GetType())
                return obj;

            if (targetType.IsAssignableFrom(obj.GetType()))
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

                var t = Array.CreateInstance(et, l.Length);

                Array.Copy(l, t, l.Length);
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
                if (obj.GetType() == typeof(string))
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

                    Match(':');

                    var field = valueType == null ? type.GetField(name) : null;
                    var fieldType = field == null ? valueType : field.FieldType;

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

                Match('}');

                return result;
            }
            else if (TryMatch('['))
            {
                if (type.GetInterface("IList") != null)
                {
                    if (type.IsArray)
                        result = Activator.CreateInstance(
                                    typeof(List<>)
                                    .MakeGenericType(type.GetElementType()));
                    else if (type.IsGenericType)
                        result = Activator.CreateInstance(
                                    typeof(List<>)
                                    .MakeGenericType(type.GenericTypeArguments));
                    else
                        result = new List<object>();

                    while (true)
                    {
                        var item = FromJson(null);

                        if (type.HasElementType)
                            item = FromJsonType(item, type.GetElementType());
                        else if (type.IsGenericType)
                            item = FromJsonType(item, type.GenericTypeArguments[0]);

                        ((IList)result).Add(item);

                        if (!TryMatch(','))
                            break;
                    }
                }

                Match(']');

                return result;
            }
            else if (TryMatch('"'))
            {
                text = "";

                while (next != '"')
                {
                    if (next == '\\')
                    {
                        MoveChar();

                        switch (next)
                        {
                            case '"':
                            case '\\':
                            case '/':
                                AppendChar();
                                break;

                            case 'b':
                                next = (char)8;
                                AppendChar();
                                break;

                            case 't':
                                next = (char)9;
                                AppendChar();
                                break;

                            case 'n':
                                next = (char)10;
                                AppendChar();
                                break;

                            case 'f':
                                next = (char)12;
                                AppendChar();
                                break;

                            case 'r':
                                next = (char)13;
                                AppendChar();
                                break;

                            case 'u':
                                var t = text;

                                text = "";

                                MoveChar();

                                while (text.Length < 4 && "0123456789abcdefABCDEF".Contains(next))
                                    AppendChar();

                                text = t + char.ConvertFromUtf32(int.Parse("0x" + text));
                                break;
                        }
                    }
                    else
                    {
                        AppendChar();
                    }
                }

                Match('"');

                return text;
            }
            else if (next == 't')
            {
                Match('t');
                Match('r');
                Match('u');
                Match('e');

                return true;
            }
            else if (next == 'f')
            {
                Match('f');
                Match('a');
                Match('l');
                Match('s');
                Match('e');

                return false;
            }
            else if (next == 'n')
            {
                Match('n');
                Match('u');
                Match('l');
                Match('l');

                return null;
            }
            else if ("-0123456789".Contains(next))
            {
                text = "";

                if (next == '-')
                    AppendChar();

                if (next == '0')
                {
                    AppendChar();
                }
                else if ("123456789".Contains(next))
                {
                    while ("0123456789".Contains(next))
                        AppendChar();
                }
                else
                    throw new FormatException("Digit expected!");

                if (next == '.')
                {
                    AppendChar();

                    while ("0123456789".Contains(next))
                        AppendChar();
                }

                if (next == 'e' || next == 'E')
                {
                    next = 'e';
                    AppendChar();

                    if (next == '+' || next == '-')
                        AppendChar();

                    while ("0123456789".Contains(next))
                        AppendChar();
                }

                return double.Parse(text, CultureInfo.InvariantCulture);
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
