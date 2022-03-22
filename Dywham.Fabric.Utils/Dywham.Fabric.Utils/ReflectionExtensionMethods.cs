using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dywham.Fabric.Utils
{
    public static class ReflectionExtensionMethods
    {
        public static bool IsConcreteDerivedTypeOf(this Type type, Type baseType)
        {
            return baseType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract;
        }

        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType) return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType) return true;

            var baseType = givenType.BaseType;

            return baseType != null && IsAssignableToGenericType(baseType, genericType);
        }

        public static bool IsConcreteDerivedTypeOf<TBase>(this Type type)
        {
            return IsConcreteDerivedTypeOf(type, typeof(TBase));
        }

        private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            PropertyInfo propInfo;

            do
            {
                propInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                type = type.BaseType;
            }
            while (propInfo == null && type != null);

            return propInfo;
        }

        public static object GetPropertyValue(this object obj, string propertyName)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            var objType = obj.GetType();
            var propInfo = GetPropertyInfo(objType, propertyName);

            if (propInfo == null)
            {
                throw new ArgumentOutOfRangeException("propertyName", $"Couldn't find property {propertyName} in type {objType.FullName}");
            }

            return propInfo.GetValue(obj, null);
        }

        public static void SetPropertyValue(this object obj, string propertyName, object val, bool silenceException = true)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            var objType = obj.GetType();
            var propInfo = GetPropertyInfo(objType, propertyName);

            if (propInfo == null && silenceException == false)
            {
                throw new ArgumentOutOfRangeException("propertyName", $"Couldn't find property {propertyName} in type {objType.FullName}");
            }

            propInfo?.SetValue(obj, val, null);
        }

        public static IEnumerable<Type> GetAllBaseTypes(this Type type)
        {
            if (type == null)
            {
                yield break;
            }

            // return all implemented or inherited interfaces
            foreach (var i in type.GetInterfaces())
            {
                yield return i;
            }

            // return all inherited types
            var currentBaseType = type.BaseType;

            while (currentBaseType != null)
            {
                yield return currentBaseType;

                currentBaseType = currentBaseType.BaseType;
            }
        }
    }
}
