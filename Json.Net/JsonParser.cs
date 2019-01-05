﻿using System;
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


        public JsonParser()
        {
        }


        [ThreadStatic]
        static JsonParser _Instance;

        public static JsonParser Instance
        {
            get
            {
                return _Instance ??
                      (_Instance = new JsonParser());
            }
        }


        public JsonParser Initialize(string json, IJsonConverter[] converters)
        {
            base.Initialize(json);
            Converters = converters;
            return this;
        }


        public JsonParser Initialize(TextReader jsonReader, params IJsonConverter[] converters)
        {
            base.Initialize(jsonReader);
            Converters = converters;
            return this;
        }
        

        StringBuilder text = new StringBuilder();

        public object FromJson(Type type)
        {
            object result;

            SkipWhite();

            if (NextChar == '{')
            {
                ReadNext();
                SkipWhite();

                if (type.IsValueType)
                    throw new FormatException("Unexpected type!");

                result = Activator.CreateInstance(type);

                var nameType = result is IDictionary ?
                    type.GenericTypeArguments[0] :
                    typeof(string);

                var valueType = result is IDictionary ?
                    type.GenericTypeArguments[1] :
                    null;

                while (NextChar!='}')
                {
                    var name = (string)FromJson(nameType);

                    SkipWhite();
                    Match(":");

                    var map = SerializerMap.GetSerializerMap(type);

                    var field = valueType == null ? map.Members.FirstOrDefault(m => m.Name == name) : null;
                    var fieldType = field == null ? valueType : field.ValueType;

                    var value = FromJson(fieldType);

                    if (field != null)
                    {
                        field.SetValue(result, value);
                    }
                    else
                    {
                        ((IDictionary)result).Add(name, value);
                    }

                    SkipWhite();

                    if (NextChar == ',')
                    {
                        ReadNext();
                        SkipWhite();
                        continue;
                    }

                    break;
                }
                
                Match("}");

                return result;
            }

            if (NextChar == '[')
            {
                ReadNext();
                SkipWhite();

                var elementType =
                    type.IsArray ?
                        type.GetElementType() :
                    type.IsGenericType ?
                        type.GenericTypeArguments[0] :
                        typeof(object);

                var list =
                    type.IsArray ?
                        new ArrayList() :
                        (IList)Activator.CreateInstance(type);

                while (NextChar != ']')
                {
                    var item = FromJson(elementType);

                    list.Add(item);

                    SkipWhite();

                    if (NextChar == ',')
                    {
                        ReadNext();
                        SkipWhite();
                        continue;
                    }

                    break;
                }

                Match("]");

                if (list is ArrayList)
                    return ((ArrayList)list).ToArray(elementType);
                else
                    return list;
            }

            if (NextChar == '"')
            {
                ReadNext();

                while (!EndOfStream && NextChar != '"')
                {
                    if (NextChar == '\\')
                    {
                        ReadNext();

                        switch (NextChar)
                        {
                            case 'b':
                            case 't':
                            case 'n':
                            case 'f':
                            case 'r':
                                text.Append(EscapeMap[NextChar]);
                                break;

                            case 'u':
                                ReadNext();

                                var unicode = "";

                                while (unicode.Length < 4 && IsHexDigit)
                                {
                                    unicode += NextChar;
                                    ReadNext();
                                }

                                text.Append(char.ConvertFromUtf32(int.Parse("0x" + unicode)));
                                continue;

                            default:
                                text.Append(NextChar);
                                break;
                        }
                    }
                    else
                        text.Append(NextChar);

                    ReadNext();
                }

                SkipWhite();
                Match("\"");

                result = text.ToString();

                text.Clear();

                var converter = Converters.FirstOrDefault(c => c.GetConvertingType() == type);

                if (converter != null)
                    return converter.Deserializer((string)result);

                if (type == typeof(DateTime)
                 || type == typeof(DateTime?))
                    return DateTime.Parse((string)result, CultureInfo.InvariantCulture);

                if (type == typeof(DateTimeOffset)
                 || type == typeof(DateTimeOffset?))
                    return DateTimeOffset.Parse((string)result, CultureInfo.InvariantCulture);

                return result;
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

                if (!(type.IsClass
                   || Nullable.GetUnderlyingType(type) != null))
                    throw new InvalidDataException("Type " + type.Name + "'s value cannot be null!");

                return null;
            }
            else if (NextChar == '-' || IsDigit)
            {
                if (NextChar == '-')
                {
                    text.Append('-');
                    ReadNext();
                }

                if (NextChar == '0')
                {
                    text.Append('0');
                    ReadNext();
                }
                else if (IsDigit)
                {
                    do
                    {
                        text.Append(NextChar);
                        ReadNext();
                    }
                    while (IsDigit);
                }
                else
                    throw new FormatException("Digit expected!");

                if (NextChar == '.')
                {
                    text.Append('.');
                    ReadNext();

                    while (IsDigit)
                    {
                        text.Append(NextChar);
                        ReadNext();
                    }
                }

                if (NextChar == 'e' || NextChar == 'E')
                {
                    text.Append('e');
                    ReadNext();

                    if (NextChar == '+' || NextChar == '-')
                    {
                        text.Append(NextChar);
                        ReadNext();
                    }

                    while (IsDigit)
                    {
                        text.Append(NextChar);
                        ReadNext();
                    }
                }

                var t = text.ToString();
                text.Clear();

                var inv = CultureInfo.InvariantCulture;

                if (type.IsEnum
                 || type == typeof(int)
                 || type == typeof(int?))
                    return int.Parse(t, inv);

                if (type == typeof(long)
                 || type == typeof(long?))
                    return long.Parse(t, inv);

                return double.Parse(t, inv);
            }

            throw new FormatException("Unexpected character! " + NextChar);
        }



    }
}
