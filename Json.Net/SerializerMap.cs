using System;
using System.Collections.Generic;
using System.Linq;
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

        static Dictionary<Type, SerializerMap> GlobalMaps = new Dictionary<Type, SerializerMap>();

        public SerializerMap(Type type)
        {
            ObjectType = type;

            Members =
                type.GetMembers(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => m.MemberType == MemberTypes.Field
                         || m.MemberType == MemberTypes.Property)
                .Select(m =>
                    m.MemberType == MemberTypes.Property ?
                    new MemberAccessor
                    {
                        Name = m.Name,
                        ValueType = ((PropertyInfo)m).PropertyType,
                        GetValue = ((PropertyInfo)m).GetValue,
                        SetValue = ((PropertyInfo)m).SetValue
                    }:
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
            if (!GlobalMaps.TryGetValue(type, out SerializerMap result))
                lock (GlobalMaps)
                {
                    if (!GlobalMaps.TryGetValue(type, out result))
                    {
                        GlobalMaps[type] = result = new SerializerMap(type);

                        foreach (var t in result.Members
                                          .Select(v => v.ValueType)
                                          .Except(GlobalMaps.Keys)
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
