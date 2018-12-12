using System;
using System.Collections.Generic;
using System.Text;

namespace Json.Net
{
    /// <summary>
    /// Converter interface
    /// </summary>
    public interface IJsonConverter
    {
        Type GetConvertingType();
        string Serializer(object obj);
        object Deserializer(string txt);
    }
}
