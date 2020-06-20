using System;
using System.Collections.Generic;
using System.Text;

namespace Json.Net
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class JsonNetIgnoreAttribute : Attribute
    {
        public JsonNetIgnoreAttribute()
        {
        }
    }
}
