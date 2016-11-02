using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Droid
{
    internal static class ReflectionUtils
    {
        public static bool DoesTypeSupportInterface(Type type, Type inter)
        {
            if (inter.IsAssignableFrom(type))
                return true;
            if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == inter))
                return true;
            return false;
        }

        public static IEnumerable<Type> TypesImplementingInterface(Type desiredType)
        {
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => DoesTypeSupportInterface(type, desiredType));

        }

        public static object GetNestedPropertyValue(this object obj, string name)
        {
            foreach (var part in name.Split('.'))
            {
                if (obj == null) { return null; }

                var type = obj.GetType();
                var info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        public static Type GetNestedProperty(this Type type, string treeProperty)
        {
            var properties = treeProperty.Split('.');
            var p = type;
            foreach (var property in properties)
            {
                p = p.GetProperty(property).PropertyType;
            }

            return p;
        }

        public static object GetDefaultValue(Type type)
        {
            if (type == typeof(string))
            {
                return string.Empty;
            }
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public static object GetNestedPropertyValue(this Type type, string treeProperty, object givenValue)
        {
            var properties = treeProperty.Split('.');
            return properties.Aggregate(givenValue, (current, property) => current.GetType().GetProperty(property).GetValue(current));
        }

    }
}