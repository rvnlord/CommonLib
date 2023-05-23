using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using CommonLib.Source.Common.Extensions.Collections;
using CommonLib.Source.Common.Utils;

namespace CommonLib.Source.Common.Extensions
{
    public static class TExtensions
    {
        public static bool EqualsAny<T>(this T o, params T[] os)
        {
            return o.EqualsAny(os.AsEnumerable());
        }

        public static bool EqualsAny<T>(this T o, IEnumerable<T> os)
        {
            var osArr = os.ToArray();
            return osArr.Length > 0 && osArr.Any(s => s is null && o is null || (s is not null && o is not null && s.Equals(o)));
        }
        
        public static bool EqualsAll<T>(this T o, params T[] os)
        {
            return os.Length > 0 && os.All(s => s.Equals(o));
        }

        public static T NullifyIf<T>(this T o, Func<T, bool> condition)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            if (o is null || condition(o))
                return (T) (object) null;
            return o;
        }

        public static T[] ToArrayOfOne<T>(this T el) => new[] { el };
        public static List<T> ToListOfOne<T>(this T el) => new() { el };

        public static IEnumerable<T> ConcatMany<T>(this T val, params IEnumerable<T>[] enums)
        {
            return IEnumerableExtensions.ConcatMany(new[] { val }, enums);
        }

        public static bool In<T>(this T o, params T[] os) => o.In(os.AsEnumerable());

        public static bool In<T>(this T o, IEnumerable<T> os)
        {
            var isImplementingIenumerable = o.IsIEnumerable();
            if (isImplementingIenumerable)
                throw new ArgumentException("You can't use `In()` with `T` being a collection, did you mean to use `AllIn()` or `AnyIn()`?");
            
            return o.EqualsAny(os);
        }
        
        public static bool IsIEnumerable<T>(this T _) => typeof(T) != typeof(string) && typeof(T).GetInterfaces().Any(i => i.Name.ContainsInvariant("IEnumerable"));

        public static bool AllIn<T>(this IEnumerable<T> en, IEnumerable<T> os)
        {
            return en.All(o => o.In(os));
        }

        public static bool AnyIn<T>(this IEnumerable<T> en, IEnumerable<T> os)
        {
            return en.Any(o => o.In(os));
        }

        public static T MapToSameType<T>(this T source) where T : class, new()
        {
            return source.MapTo(new T());
        }

        public static TDest MapTo<TSource, TDest>(this TSource source, TDest dest, IMapper mapper = null)
        {
            mapper ??= AutoMapperUtils.Instance.Mapper;
            if (mapper == null)
                throw new AutoMapperConfigurationException("Automapper is not configured, call Configure() method of AutoMapperUtils");

            mapper.Map(source, dest);
            return dest;
        }

        public static T MapperCopy<T>(this T src) where T : class, new() => src.MapToSameType();

        public static bool IsNullable<T>(this T t)
        {
            if (t == null) return true; 
            var type = typeof(T);
            if (!type.IsValueType) return true; 
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static Type EnsureNonNullable(this Type type)
        {
            return type.IsNullable() ? type.GenericTypeArguments.FirstOrDefault() ?? type : type;
        }
        
        public static T DeepCopy<T>(this T original) => (T)((object)original).DeepCopy();

        public static T EnsureWin7<T>(this T o)
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(7))
                throw new PlatformNotSupportedException();
            return o;
        }

        public static TSource Nullify<TSource, TProperty>(this TSource src, Expression<Func<TSource, TProperty>> prop)
        {
            if (src is null)
                throw new ArgumentNullException(nameof(src));
            if (prop is null)
                throw new ArgumentNullException(nameof(prop));
            if (prop.Body is not MemberExpression memberExpression)
                throw new ArgumentException("Invalid expression", nameof(prop));
            if (memberExpression.Member is not PropertyInfo pi)
                throw new ArgumentException("Invalid expression", nameof(prop));
        
            pi.SetValue(src, null);
            return src;
        }
    }
}
