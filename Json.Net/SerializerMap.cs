using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Json.Net
{
    public class MemberAccessor
    {
        public string Name;
        public Type ValueType;
        public Func<object, object> GetValue;
        public Action<object, object> SetValue;
    }


    public class SerializerMap
    {
        public readonly Type ObjectType;
        public readonly MemberAccessor[] Members;

        [ThreadStatic]
        static SerializerMap[] _GlobalMaps;

        static SerializerMap[] GlobalMaps
        {
            get
            {
                return _GlobalMaps ??
                      (_GlobalMaps = new SerializerMap[0]);
            }
        }

        string[] IgnoredAttributes = new[] {
            "JsonIgnoreAttribute",
            "JsonNetIgnoreAttribute",
            "NonSerializedAttribute",
            "XmlIgnoreAttribute"
        };

        public SerializerMap(Type type)
        {
            ObjectType = type;

            if (type == typeof(object)
             || type == typeof(ExpandoObject))
            {
                Members = new MemberAccessor[0];                
            }
            else
                Members =
                    type.GetMembers(BindingFlags.Instance | BindingFlags.Public)
                    .Where(m => m.MemberType == MemberTypes.Field
                             || (m.MemberType == MemberTypes.Property
                                 && type.GetProperty(m.Name).CanRead
                                 && !m.GetCustomAttributes().Select(a => a.GetType().Name)
                                    .Intersect(IgnoredAttributes)
                                    .Any()))
                    .Select(m =>
                        m.MemberType == MemberTypes.Property ?
                        new MemberAccessor
                        {
                            Name = m.Name,
                            ValueType = ((PropertyInfo)m).PropertyType,
                            GetValue = ((PropertyInfo)m).GetValue,
                            SetValue = ((PropertyInfo)m).SetValue
                        } :
                        new MemberAccessor
                        {
                            Name = m.Name,
                            ValueType = ((FieldInfo)m).FieldType,
                            GetValue = ((FieldInfo)m).GetValue,
                            SetValue = ((FieldInfo)m).SetValue
                        })
                    .ToArray();
        }
        

        public static SerializerMap GetSerializerMap(Type type)
        {
            SerializerMap result;

            if ((result = GlobalMaps.FirstOrDefault(m => m.ObjectType == type)) == null)
                lock (_GlobalMaps)
                {
                    if ((result = _GlobalMaps.FirstOrDefault(m => m.ObjectType == type)) == null)
                    {
                        var l = _GlobalMaps.Length;
                        Array.Resize(ref _GlobalMaps, l + 1);

                        _GlobalMaps[l] = result = new SerializerMap(type);

                        foreach (var t in result.Members
                                          .Select(v => v.ValueType)
                                          .Where(t => t != typeof(string)
                                                   && (t.IsClass || (t.IsValueType && !t.IsPrimitive)))
                                          .Except(GlobalMaps.Select(m => m.ObjectType))
                                          .Distinct())
                        {
                            GetSerializerMap(t);
                        }
                    }
                }

            return result;
        }
        
    }
}
