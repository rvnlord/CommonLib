using System;
using System.Collections.Generic;
using System.Linq;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions;
using CommonLib.Source.Common.Utils.UtilClasses;

namespace CommonLib.Source.Common.Utils.TypeUtils
{
    public static class EnumUtils
    {
        public static List<DdlItem> EnumToDdlItems<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<int>().Select(i => new DdlItem(i, Enum.GetName(typeof(T), i)?.ReplaceInvariant("_", " ").Trim())).ToList();
        }

        public static List<DdlItem<TValue>> EnumToTypedDdlItems<TValue>()
        {
            return GetValues<TValue>().Select(v => new DdlItem<TValue>(v, v.EnumToString())).ToList();
        }

        public static List<DdlItem> EnumToDdlItems(this Type type)
        {
            type = type.EnsureNonNullable();
            return type == null ? null : Enum.GetValues(type).Cast<int>().Select(i => new DdlItem(i, Enum.GetName(type, i)?.ReplaceInvariant("_", " ").Trim())).ToList();
        }

        public static List<DdlItem> EnumToDdlItems<T>(Func<T, string> customNamesConverter)
        {
            return EnumToDdlItems<T>()
                .Select(item =>
                {
                    if (item?.Index == null)
                        throw new NullReferenceException(nameof(item));

                    return new DdlItem(
                        item.Index,
                        customNamesConverter((T)Enum.ToObject(typeof(T), item.Index)));
                })
                .ToList();
        }

        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T).EnsureNonNullable()).Cast<T>();
        }

        public static IEnumerable<T> GetValuesOrNull<T>()
        {
            if (!typeof(T).IsEnum)
                return null;
            var type = typeof(T).EnsureNonNullable();
            return type == null ? null : Enum.GetValues(type).Cast<T>();
        }

        public static bool IsEnum<T>()
        {
            var type = typeof(T).EnsureNonNullable();
            var isEnum = type.IsEnum;
            return isEnum;
            //var isEnum = false;
            //try
            //{
            //    _ = Enum.GetValues(type);
            //    isEnum = true;
            //}
            //catch (ArgumentException)
            //{
            //    // ignored
            //}

            //return isEnum;
        }
    }
}
