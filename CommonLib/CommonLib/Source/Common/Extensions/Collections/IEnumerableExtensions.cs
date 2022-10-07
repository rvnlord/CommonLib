using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CommonLib.Source.Common.Converters;
using MoreLinq;
using Truncon.Collections;

namespace CommonLib.Source.Common.Extensions.Collections
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> source, int n)
        {
            if (n == 0)
                yield return Enumerable.Empty<T>();


            var count = 1;
            var arr = source.ToArray();
            foreach (var item in arr)
            {
                foreach (var innerSequence in arr.Skip(count).Combinations(n - 1))
                    yield return new[] { item }.Concat(innerSequence);
                count++;
            }
        }

        public static string JoinAsString<T>(this IEnumerable<T> enumerable, string strBetween = "")
        {
            return string.Join(strBetween, enumerable.ToArray());
        }

        public static string JoinAsString<T>(this IEnumerable<T> enumerable, string[] substrings)
        {
            if (substrings == null)
                throw new ArgumentNullException(nameof(substrings));
            
            var strings = enumerable.Cast<string>().ToArray();

            if (strings.Length == 1)
                return strings[0]; // whatever is the separator we have nothing to separate
            if (strings.Length - 1 != substrings.Length)
                throw new ArgumentOutOfRangeException(nameof(substrings));
            return strings.Select((s, i) => s + (i < substrings.Length ? substrings[i] : "")).Aggregate((s1, s2) => s1 + s2);
        }

        public static int IndexOf_<T>(this IEnumerable<T> en, T el)
        {
            if (en == null)
                throw new ArgumentNullException(nameof(en));

            var i = 0;
            foreach (var item in en)
            {
                if (Equals(item, el)) return i;
                i++;
            }
            return -1;
        }
        public static async Task<int> IndexOfAsync<T>(this IEnumerable<T> en, T el) => await Task.Run(() => en.IndexOf_(el));

        public static OrderedDictionary<TSource, int> IndexOfEach<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> colToIndIndicesOf)
        {
            var indices = new OrderedDictionary<TSource, int>();
            var sourceArr = source.ToArray();
            foreach (var el in colToIndIndicesOf)
                indices[el] = sourceArr.IndexOf_(el);
            return indices;
        }

        public static async Task<OrderedDictionary<TSource, int>> IndexOfEachAsync<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> colToIndIndicesOf) => await Task.Run(() => source.IndexOfEach(colToIndIndicesOf));

        public static int[] IndexOfEach<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> selector)
        {
            var sourceArr = source.ToArray();
            return sourceArr.Select((e, i) => new KeyValuePair<int, bool>(i, selector(e))).Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();
        }

        public static async Task<int[]> IndexOfEachAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> selector) => await Task.Run(() => source.IndexOfEach(selector));

        public static int IndexOf_(this IEnumerable en, object el)
        {
            if (en == null)
                throw new ArgumentNullException(nameof(en));

            var i = 0;
            foreach (var item in en)
            {
                if (Equals(item, el)) return i;
                i++;
            }
            return -1;
        }

        public static int IndexOf_<T>(this IEnumerable<T> sourceCol, IEnumerable<T> subCol, int start = 0, int length = -1)
        {
            var sourceArray = sourceCol.ToArray();
            var sourceArrayLength = sourceArray.Length;
            var subArray = subCol.ToArray();
            var subArrayLength = subArray.Length;

            if (subArrayLength <= 0)
                return -1;
            if (length == -1)
                length = sourceArrayLength;

            while (length >= subArrayLength)
            {
                var index = Array.IndexOf(sourceArray, subArray[0], start, length - subArrayLength + 1);
                if (index == -1)
                    return -1;

                int i, p;
                for (i = 0, p = index; i < subArrayLength; i++, p++)
                    if (!sourceArray[p].Equals(subArray[i]))
                        break;

                if (i == subArrayLength)
                    return index;
                
                length -= index - start + 1;
                start = index + 1;
            }
            return -1;
        }

        public static async Task<int> IndexOfAsync<T>(this IEnumerable<T> sourceCol, IEnumerable<T> subCol, int start = 0, int length = -1)
            => await Task.Run(() => sourceCol.IndexOf_(subCol, start, length));

        public static T LastOrNull<T>(this IEnumerable<T> enumerable) where T : class
        {
            var en = enumerable as T[] ?? enumerable.ToArray();
            return en.Any() ? en.Last() : (T)Convert.ChangeType(null, typeof(T), CultureInfo.InvariantCulture);
        }

        public static IEnumerable<T> ConcatMany<T>(this IEnumerable<T> enumerable, params IEnumerable<T>[] enums)
        {
            return enumerable.Concat(enums.SelectMany(x => x));
        }

        public static IEnumerable<TSource> WhereByMany<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> selector,
            IEnumerable<TKey> matches) where TSource : class
        {
            return source.Where(e => matches.Any(sel => Equals(sel, selector(e))));
        }

        public static IEnumerable<TSource> OrderByWith<TSource, TResult>(this IEnumerable<TSource> en, Func<TSource, TResult> selector, IEnumerable<TResult> order)
        {
            return order.Select(el => en.Select(x => new { x, res = selector(x) }).Single(e => Equals(e.res, el)).x);
        }

        public static IEnumerable<T> OrderWith<T>(this IEnumerable<T> enumerable, IEnumerable<int> orderPattern)
        {
            var enArr = enumerable.ToArray();
            var opArr = orderPattern.ToArray();
            if (enArr.Length != opArr.Length)
                throw new ArgumentException($"{nameof(enumerable)}.Count() != {nameof(orderPattern)}.Count()");
            return opArr.Select(i => enArr[i]);
        }

        public static IEnumerable<T> OrderByAnother<T, TProp>(this IEnumerable<T> en, Func<T, TProp> selector, IEnumerable<T> anotherEn)
        {
            var arr = (en ?? throw new NullReferenceException(nameof(en))).ToArray();
            if (anotherEn == null)
                throw new NullReferenceException(nameof(anotherEn));
            var list = new List<T>();

            foreach (var item in anotherEn)
            {
                var existingItem = arr.SingleOrDefault(i => selector(item).Equals(selector(i)));
                if (existingItem != null)
                    list.Add(existingItem);
            }

            return list;
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T el)
        {
            return enumerable.Except(new[] { el });
        }

        public static List<object> DisableControls(this IEnumerable<object> controls)
        {
            if (controls == null)
                throw new ArgumentNullException(nameof(controls));

            var disabledControls = new List<object>();
            foreach (var c in controls)
            {
                var piIsEnabled = c.GetType().GetProperty("IsEnabled");
                var isEnabled = (bool?)piIsEnabled?.GetValue(c);
                if (isEnabled == true)
                {
                    piIsEnabled.SetValue(c, false);
                    disabledControls.Add(c);
                }
            }
            return disabledControls;
        }

        public static bool AllEqual<T>(this IEnumerable<T> en)
        {
            var arr = en.ToArray();
            return arr.All(el => Equals(el, arr.First()));
        }

        public static NameValueCollection ToNameValueCollection(this IEnumerable<KeyValuePair<string, string>> en)
        {
            if (en == null)
                throw new ArgumentNullException(nameof(en));

            var nvc = new NameValueCollection();
            foreach (var q in en)
                nvc.Add(q.Key, q.Value);
            return nvc;
        }

        public static IEnumerable<T> Duplicates<T>(this IEnumerable<T> en, bool distinct = true)
        {
            var duplicates = en.GroupBy(s => s).SelectMany(grp => grp.Skip(1));
            return distinct ? duplicates.Distinct() : duplicates;
        }

        public static List<TDest> MapCollectionTo<TDest, TSource>(this IEnumerable<TSource> sourceCollection)
            where TSource : class
            where TDest : class, new()
        {
            return sourceCollection.Select(source => source.MapTo(new TDest())).ToList();
        }

        public static List<TDest> MapCollectionToSameType<TDest>(this IEnumerable<TDest> source) where TDest : class, new()
        {
            return source.Select(srcEl => srcEl.MapToSameType()).ToList();
        }

        public static List<T> MapperCopyCollection<T>(this IEnumerable<T> src) where T : class, new() => src.MapCollectionToSameType();

        public static bool ContainsAll<T>(this IEnumerable<T> en1, IEnumerable<T> en2)
        {
            var arr1 = en1.Distinct().ToArray();
            var arr2 = en2.Distinct().ToArray();
            return arr1.Intersect(arr2).Count() == arr2.Length;
        }

        public static bool ContainsAny<T>(this IEnumerable<T> en1, IEnumerable<T> en2)
        {
            var arr1 = en1.Distinct().ToArray();
            var arr2 = en2.Distinct().ToArray();
            return arr1.Intersect(arr2).Any();
        }

        public static bool ContainsAll<T>(this IEnumerable<T> en1, params T[] en2)
        {
            return en1.ContainsAll(en2.AsEnumerable());
        }

        public static bool ContainsAny<T>(this IEnumerable<T> en1, params T[] en2)
        {
            return en1.ContainsAny(en2.AsEnumerable());
        }

        public static bool ContainsAll(this IEnumerable<string> en1, IEnumerable<string> en2)
        {
            var arr1 = en1.Select(x => x.ToUpperInvariant()).Distinct().ToArray();
            var arr2 = en2.Select(x => x.ToUpperInvariant()).Distinct().ToArray();
            return arr1.Intersect(arr2).Count() == arr2.Length;
        }

        public static bool ContainsAny(this IEnumerable<string> en1, IEnumerable<string> en2)
        {
            var arr1 = en1.Select(x => x.ToUpperInvariant()).Distinct().ToArray();
            var arr2 = en2.Select(x => x.ToUpperInvariant()).Distinct().ToArray();
            return arr1.Intersect(arr2).Any();
        }

        public static bool ContainsAll(this IEnumerable<string> en1, params string[] en2)
        {
            return en1.ContainsAll(en2.AsEnumerable());
        }

        public static bool ContainsAny(this IEnumerable<string> en1, params string[] en2)
        {
            return en1.ContainsAny(en2.AsEnumerable());
        }

        public static bool CollectionEqual<T>(this IEnumerable<T> col1, IEnumerable<T> col2)
        {
            if (col1 == null && col2 == null)
                return true;

            var arr1 = col1?.ToArray();
            var arr2 = col2.ToArray();
            return arr1?.Length == arr2.Length && arr1.Intersect(arr2).Count() == arr1.Length;
        }

        public static IEnumerable<TSource> ExceptBy<TSource, TSelector>(this IEnumerable<TSource> en, TSource el, Func<TSource, TSelector> selector)
        {
            return en.ExceptBy(el.ToEnumerable(), selector);
        }

        public static T FirstOrNull<T>(this IEnumerable<T> values) where T : class
        {
            return values.DefaultIfEmpty(null).FirstOrDefault();
        }

        public static T FirstOrNull<T>(this IEnumerable<T> values, Func<T, bool> selector) where T : class
        {
            var arrValues = values as T[] ?? values.ToArray();
            return arrValues.Any() ? arrValues.First(selector) : null;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> en)
        {
            return new ObservableCollection<T>(en);
        }

        public static string EnvVar(this string[] args, string var)
        {
            return args.FirstOrNull(a => a.StartsWithInvariant($"{var}="))?.AfterFirst($"{var}=");
        }

        public static IEnumerable<T> SliceExcl<T>(this IEnumerable<T> en, int startIndex, int endIndexExcluded)
        {
            return en.Skip(startIndex - 1).Take(endIndexExcluded - startIndex);
        }

        public static IEnumerable<T> Between<T>(this IEnumerable<T> en, int startIndex, int endIndexExcluded)
        {
            return en.Skip(startIndex - 1).Take(endIndexExcluded - startIndex + 1);
        }

        public static IEnumerable<T> AppendEl<T>(this IEnumerable<T> en, T el)
        {
            return MoreEnumerable.Append(en, el);
        }

        public static IEnumerable<T> PrependEl<T>(this IEnumerable<T> en, T el)
        {
            return MoreEnumerable.Prepend(en, el);
        }

        public static IEnumerable<TSource> SkipLast_<TSource>(this IEnumerable<TSource> source, int n) => MoreEnumerable.SkipLast(source, n);
        
        public static IEnumerable<TSource> TakeLastWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null) 
                throw new ArgumentNullException(nameof(predicate));

            var buffer = new List<TSource>();
            foreach (var item in source)
                if (predicate(item))
                    buffer.Add(item);
                else
                    buffer.Clear();

            foreach (var item in buffer)
                yield return item;
        }

        public static IEnumerable<TSource> SkipLastWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var buffer = new List<TSource>();

            foreach (var item in source)
            {
                if (predicate(item))
                    buffer.Add(item);
                else
                {
                    if (buffer.Count > 0)
                    {
                        foreach (var bufferedItem in buffer)
                            yield return bufferedItem;

                        buffer.Clear();
                    }

                    yield return item;
                }
            }
        }

        public static T FirstOfAnyOrDefault<T>(this IEnumerable<T> en, params Func<T, bool>[] conditions)
        {
            var list = en.ToList();
            foreach (var c in conditions)
            {
                var fOrD = list.FirstOrDefault(c);
                if (fOrD?.Equals(default(T)) != true)
                    return fOrD;
            }

            return default;
        }

        public static IEnumerable<T> SkipLastN<T>(this IEnumerable<T> en, int n) => Enumerable.SkipLast(en, n);

        public static bool Any_([NotNull] this IEnumerable source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var enumerator = source.GetEnumerator();
            try
            {
                if (enumerator.MoveNext())
                    return true;
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                    disposable.Dispose();
            }
            return false;
        }

        public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> selector)
        {
            var sourceArr = source.ToArray();
            return sourceArr.Where(selector).Count() == sourceArr.Length;
        }

        public static ValueTask<T> SingleAsync<T>(this IEnumerable<T> en, Func<T, Task<bool>> selector)
        {
            return en.ToAsyncEnumerable().SingleAwaitAsync(async x => await selector(x).ConfigureAwait(false));
        }

        public static ValueTask<T> SingleAsync<T>(this IEnumerable<T> en) => en.SingleAsync(x => Task.FromResult(true));

        public static ValueTask<T> SingleOrDefaultAsync<T>(this IEnumerable<T> en, Func<T, bool> selector)
        {
            return en.ToAsyncEnumerable().SingleOrDefaultAwaitAsync(async x => await ValueTask.FromResult(selector(x)).ConfigureAwait(false));
        }

        public static ValueTask<T> SingleOrDefaultAsync<T>(this IEnumerable<T> en) => en.SingleOrDefaultAsync(_ => true);

        public static IEnumerable<T> RemoveNulls<T>(this IEnumerable<T> en)
        {
            return en.Where(x => x != null);
        }

        public static IEnumerable<string> RemoveEmptyEntries(this IEnumerable<string> en)
        {
            return en.Where(x => !x.IsNullOrEmpty());
        }

        public static IEnumerable<TSource> DistinctBy_<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => MoreEnumerable.DistinctBy(source, keySelector);

        public static IExtremaEnumerable<TSource> MaxBy_<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => MoreEnumerable.MaxBy(source, keySelector);

        public static IExtremaEnumerable<TSource> MinBy_<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => MoreEnumerable.MinBy(source, keySelector);

        public static TSource MaxOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue = default) => source.DefaultIfEmpty(defaultValue).Max();
        public static TSource MinOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue = default) => source.DefaultIfEmpty(defaultValue).Min();

        public static IEnumerable<TSource> TakeLast_<TSource>(this IEnumerable<TSource> source, int count) => MoreEnumerable.TakeLast(source, count);
        
        public static async Task<IEnumerable<TSource>> WhereAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> selector) => await Task.Run(() => source.Where(selector));
        public static async Task<IEnumerable<TSource>> WhereAsync<TSource>(this Task<IEnumerable<TSource>> source, Func<TSource, bool> selector) => await (await source).WhereAsync(selector);

        public static IEnumerable<TSource> Prepend_<TSource>(this IEnumerable<TSource> source, TSource element) => MoreEnumerable.Prepend(source, element);
        public static IEnumerable<TSource> Append_<TSource>(this IEnumerable<TSource> source, TSource element) => MoreEnumerable.Append(source, element);

        public static T Second<T>(this IEnumerable<T> en) => en.ElementAt(1);
    }
}
