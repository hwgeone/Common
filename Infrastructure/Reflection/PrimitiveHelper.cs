using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Infrastructure.Reflection
{
    public static class PrimitiveHelper
    {
        //public static TValue GetOrDefault<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        //{
        //    dictionary.TryGetValue(key, out TValue value);
        //    return value;
        //}

        //private static IEnumerable<MemberInfo> GetAllMembers(this Type type) =>
        //    type.GetTypeInheritance().Concat(type.GetTypeInfo().ImplementedInterfaces).SelectMany(i => i.GetDeclaredMembers());

        //public static MemberInfo GetInheritedMember(this Type type, string name) => type.GetAllMembers().FirstOrDefault(mi => mi.Name == name);

        //public static MethodInfo GetInheritedMethod(Type type, string name)
        //    => type.GetInheritedMember(name) as MethodInfo ?? throw new ArgumentOutOfRangeException(nameof(name), $"Cannot find method {name} of type {type}.");

        //public static MemberInfo GetFieldOrProperty(Type type, string name) 
        //    => type.GetInheritedMember(name) ?? throw new ArgumentOutOfRangeException(nameof(name), $"Cannot find member {name} of type {type}.");

        public static bool IsNullableType(Type type) 
        { 
            return type.IsGenericType();
        }

        public static Type GetTypeOfNullable(Type type) 
        { return type.GetTypeInfo().GenericTypeArguments[0];
        } 

        public static bool IsCollectionType(Type type) 
        {
            return type.ImplementsGenericInterface(typeof(ICollection<>));
        }

        public static bool IsEnumerableType(Type type) 
        {
           return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static bool IsQueryableType(Type type)
        {
            return typeof(IQueryable).IsAssignableFrom(type);
        }

        public static bool IsListType(Type type)
        {
            return typeof(IList).IsAssignableFrom(type);
        }

        public static bool IsListOrDictionaryType(Type type)
        {
            return type.IsListType() || type.IsDictionaryType();
        }

        public static bool IsDictionaryType(Type type) 
        {
            return type.ImplementsGenericInterface(typeof(IDictionary<,>));
        }

        public static bool ImplementsGenericInterface(Type type, Type interfaceType)
        {
            return type.IsGenericType(interfaceType)
                   || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => @interface.IsGenericType(interfaceType));
        }

        public static bool IsGenericType(Type type, Type genericType)
        {
            return type.IsGenericType() && type.GetGenericTypeDefinition() == genericType;
        }

        public static Type GetIEnumerableType(Type type)
        {return type.GetGenericInterface(typeof(IEnumerable<>));
        }

        public static Type GetDictionaryType(Type type)
        {return type.GetGenericInterface(typeof(IDictionary<,>));
        }

        public static Type GetGenericInterface(Type type, Type genericInterface)
        {
            return type.IsGenericType(genericInterface)
                ? type
                : type.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(t => t.IsGenericType(genericInterface));
        }

        public static Type GetGenericElementType(Type type)
        {
            return type.HasElementType ? type.GetElementType() : type.GetTypeInfo().GenericTypeArguments[0];
        }
    }
}
