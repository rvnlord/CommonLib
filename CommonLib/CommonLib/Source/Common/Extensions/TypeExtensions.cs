using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using CommonLib.Source.Common.Converters;

namespace CommonLib.Source.Common.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsIEnumerableType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetInterface(nameof(IEnumerable)) != null;
        }

        public static bool IsIListType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetInterface(nameof(IList)) != null;
        }

        public static bool IsIDictionaryType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetInterface(nameof(IDictionary)) != null;
        }

        public static bool IsObservableCollectionType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ObservableCollection<>);
        }

        public static string GetPropertyDisplayName(this Type modelType, string propertyName)
        {
            if (modelType == null)
                throw new NullReferenceException(nameof(modelType));

            if (string.IsNullOrEmpty(propertyName))
                return string.Empty;

            var attr = (DisplayNameAttribute)modelType.GetProperty(propertyName)?.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();

            if (attr != null)
                return attr.DisplayName;

            var metadataType = (MetadataTypeAttribute)modelType.GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault();

            if (metadataType == null)
                return propertyName;

            var property = metadataType.MetadataClassType.GetProperty(propertyName);
            if (property != null)
                attr = (DisplayNameAttribute)property.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();

            return attr != null ? attr.DisplayName : propertyName;
        }

        public static MethodInfo GetMethodWithLinq(this Type staticType, string methodName, params Type[] paramTypes)
        {
            if (staticType == null)
                throw new NullReferenceException(nameof(staticType));

            var methods = staticType.GetMethods().Where(method => method.Name == methodName 
                && method.GetParameters()
                    .Select(parameter => parameter.ParameterType)
                    .Select(type => type.IsGenericType 
                        ? type.GetGenericTypeDefinition() 
                        : type).SequenceEqual(paramTypes));

            try
            {
                return methods.SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                throw new AmbiguousMatchException();
            }
        }

        public static Dictionary<string, T> GetConstants<T>(this Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                .Select(fi => new KeyValuePair<string, T>(fi.Name, (T) fi.GetRawConstantValue())).ToDictionary();
        }

        public static IEnumerable<Type> GetImplementingTypes(this Type itype) 
            => AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                .Where(t => t.GetInterfaces().Contains(itype));
    }
}
