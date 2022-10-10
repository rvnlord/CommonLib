using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using CommonLib.Source.Common.Extensions.Collections;
using CommonLib.Source.Common.Utils.UtilClasses.Comparers;

namespace CommonLib.Source.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static void ThrowIfNull(this object o)
        {
            if (o == null)
                throw new ArgumentNullException(nameof(o));
        }

        public static T GetProperty<T>(this object src, string propName)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            return (T)src.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)?.GetValue(src, null);
        }
        
        public static T GetPropertyOrNull<T>(this object src, string propName) where T : class
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            return src.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)?.GetValue(src, null) as T;
        }

        public static object GetProperty(this object src, string propName) => src.GetProperty<object>(propName);
        public static object GetPropertyOrNull(this object src, string propName) => src.GetPropertyOrNull<object>(propName);

        public static void SetProperty<T>(this object src, string propName, T propValue)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            src.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)?.SetValue(src, propValue);
        }

        public static T GetField<T>(this object src, string fieldName)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            return (T)src.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)?.GetValue(src);
        }

        public static T GetFieldByPartialName<T>(this object src, string fieldName)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            return (T)src.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Single(f => f.Name.ContainsInvariant(fieldName)).GetValue(src);
        }

        public static void SetField<T>(this object src, string fieldName, T fieldValue)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            src.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)?.SetValue(src, fieldValue);
        }

        public static T CastTo<T>(this object o) => (T)o;

        public static dynamic CastToReflected(this object o, Type type)
        {
            var methodInfo = typeof(ObjectExtensions).GetMethod(nameof(CastTo), BindingFlags.Static | BindingFlags.Public);
            var genericArguments = new[] { type };
            var genericMethodInfo = methodInfo?.MakeGenericMethod(genericArguments);
            return genericMethodInfo?.Invoke(null, new[] { o });
        }

        public static bool IsPrimitive(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (type == typeof(string)) return true;
            return type.IsValueType & type.IsPrimitive;
        }

        public static object DeepCopy(this object originalObject) => InternalCopy(originalObject, new Dictionary<object, object>(new CustomReferenceEqualityComparer()));

        private static object InternalCopy(object originalObject, IDictionary<object, object> visited)
        {
            if (originalObject == null) return null;
            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect)) return originalObject;
            if (visited.ContainsKey(originalObject)) return visited[originalObject];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
            var cloneObject = LibConfig.CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    var clonedArray = (Array)cloneObject;
                    clonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }

            }
            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType == null) 
                return;

            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
            CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (var fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false) continue;
                if (IsPrimitive(fieldInfo.FieldType)) continue;
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }

        public static string[] GetPropertyNames(this object o) => o.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Select(p => p.Name).ToArray();

        public static string GetDescriptionOrNull(this object o)
        {
            var attr = Attribute.GetCustomAttribute(o.GetType(), typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attr?.Description;
        }

        public static string GetPropertyDescriptionOrNull(this object model, string propertyName)
        {
            var pi = model.GetType().GetProperty(propertyName);
            if (pi is null)
                return null;
            var attr = Attribute.GetCustomAttribute(pi, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attr?.Description;
        }
    }
}
