using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions.Collections;
using HtmlAgilityPack;
using MoreLinq;
using Nager.PublicSuffix;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static CommonLib.Source.LibConfig;

namespace CommonLib.Source.Common.Extensions
{
    public static class StringExtensions
    {
        public static int GetHashCodeInvariant(this string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return s.GetHashCode(StringComparison.InvariantCulture);
        }

        public static string ReplaceInvariant(this string s, string src, string dest)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return s.Replace(src, dest, StringComparison.InvariantCulture);
        }

        public static bool StartsWithInvariant(this string s, string start)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return s.StartsWith(start, StringComparison.InvariantCulture);
        }

        public static bool EndsWithInvariant(this string s, string end)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return s.EndsWith(end, StringComparison.InvariantCulture);
        }

        public static bool IsSpace(this char c)
        {
            return c switch
            {
                ' ' => true,
                '\t' => true,
                '\n' => true,
                '\v' => true,
                '\f' => true,
                '\r' => true,
                _ => false
            };
        }

        public static bool HasValueBetween(this string str, string start, string end)
        {
            return !str.IsNullOrEmpty() && !string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end) &&
                   str.ContainsInvariant(start) &&
                   str.ContainsInvariant(end) &&
                   str.IndexOf(start, StringComparison.Ordinal) < str.IndexOf(end, StringComparison.Ordinal);
        }

        public static string Between(this string str, string start, string end)
        {
            return str.AfterFirst(start).BeforeFirst(end);
        }
        
        public static string BetweenOrNull(this string str, string start, string end)
        {
            if (str.AfterFirstOrNull(start) == null)
                return null;
            if (str.BeforeFirstOrNull(end) == null)
                return null;

            return str.AfterFirst(start).BeforeFirst(end);
        }

        public static string BetweenOrWhole(this string str, string start, string end)
        {
            if (str.AfterFirstOrNull(start) == null)
                return str;
            if (str.BeforeFirstOrNull(end) == null)
                return str;

            return str.AfterFirst(start).BeforeFirst(end);
        }

        public static string TakeUntil(this string str, string end)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return str.Split(new[] { end }, StringSplitOptions.None)[0];
        }

        public static string RemoveHTMLSymbols(this string str)
        {
            return str.ReplaceInvariant("&nbsp;", "")
                .ReplaceInvariant("&amp;", "");
        }

        public static bool IsNullWhiteSpaceOrDefault(this string str, string defVal)
        {
            return string.IsNullOrWhiteSpace(str) || str == defVal;
        }

        public static bool ContainsInvariant(this string s, string substring)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return s.Contains(substring, StringComparison.InvariantCulture);
        }

        public static bool ContainsInvariant(this string s, char c)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return s.Contains(c, StringComparison.InvariantCulture);
        }

        public static bool ContainsAny(this string str, params string[] strings)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            if (strings == null || strings.Length == 0) return false;
            return strings.Any(str.Contains);
        }

        public static string Remove(this string str, string substring)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return str.ReplaceInvariant(substring, "");
        }

        public static string RemoveMany(this string str, params string[] substrings)
        {
            return substrings.Aggregate(str, (current, substring) => current.Remove(substring));
        }

        public static string[] SplitIgnoreCase(this string s, string separator)
        {
            return Regex.Split(s, Regex.Escape(separator), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        public static string[] SplitByMany(this string s, params string[] separators)
        {
            return Regex.Split(s, separators.Select(Regex.Escape).JoinAsString("|"), RegexOptions.CultureInvariant);
        }

        public static string[] SplitByManyIgnoreCase(this string s, params string[] separators)
        {
            return Regex.Split(s, separators.Select(Regex.Escape).JoinAsString("|"), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        public static string[] SplitByFirst(this string str, params string[] strings)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            foreach (var s in strings)
                if (str.ContainsInvariant(s))
                    return str.Split(s);
            return new[] { str };
        }

        public static IEnumerable<string> SplitInPartsOf(this string s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException(@"Part length has to be positive.", nameof(partLength));

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

        public static string[] SameWords(this string str, string otherStr, bool casaeSensitive = false, string splitBy = " ", int minWordLength = 1)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (otherStr == null)
                throw new ArgumentNullException(nameof(otherStr));

            var str1Arr = str.Split(splitBy);
            var str2Arr = otherStr.Split(splitBy);
            var intersection = str1Arr.Intersect(str2Arr, casaeSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase).Where(w => w.Length >= minWordLength);
            return intersection.ToArray();
        }

        public static string[] SameWords(this string str, string[] otherStrings, bool casaeSensitive, string splitBy = " ", int minWordLength = 1)
        {
            if (otherStrings == null)
                throw new ArgumentNullException(nameof(otherStrings));

            var sameWords = new List<string>();

            foreach (var otherStr in otherStrings)
                sameWords.AddRange(str.SameWords(otherStr, casaeSensitive, splitBy, minWordLength));

            return sameWords.Distinct().ToArray();
        }

        public static string[] SameWords(this string str, params string[] otherStrings)
        {
            return str.SameWords(otherStrings, false, " ", 1);
        }

        public static bool HasSameWords(this string str, string otherStr, bool caseSensitive = false, string splitBy = " ", int minWordLength = 1)
        {
            return str.SameWords(otherStr, caseSensitive, splitBy, minWordLength).Any();
        }

        public static bool HasSameWords(this string str, string[] otherStrings, bool caseSensitive, string splitBy = " ", int minWordLength = 1)
        {
            return str.SameWords(otherStrings, caseSensitive, splitBy, minWordLength).Any();
        }

        public static bool HasSameWords(this string str, params string[] otherStrings)
        {
            return str.SameWords(otherStrings, false, " ", 1).Any();
        }

        public static bool IsDouble(this string str)
        {
            return str.TryToDouble() != null;
        }

        public static bool StartsWithAny(this string str, params string[] strings)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return strings.Any(str.StartsWith);
        }

        public static bool StartsWithAnyIgnoreCase(this string str, params string[] strings)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return strings.Any(str.StartsWithIgnoreCase);
        }

        public static bool StartsWithDigit(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return char.IsDigit(str[0]);
        }

        public static bool EndsWithAny(this string str, params string[] strings)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return strings.Any(str.EndsWithInvariant);
        }

        public static bool EndsWithAnyIgnoreCase(this string str, params string[] strings)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return strings.Any(str.EndsWithIgnoreCase);
        }

        public static string EndsWithAnyAndGetOrNull(this string str, params string[] strings)
        {
            return strings.FirstOrDefault(str.EndsWithInvariant);
        }

        public static bool ContainsAll(this string str, params string[] strings)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            if (strings == null || strings.Length == 0) return false;
            return strings.All(str.Contains);
        }

        public static string RemoveWord(this string str, string word, string separator = " ")
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return string.Join(separator, str.Split(separator).Where(w => w != word));
        }

        public static string RemoveWords(this string str, params string[] words)
        {
            return words.Aggregate(str, (current, w) => current.RemoveWord(w));
        }

        public static bool IsUri(this string str)
        {
            return Uri.TryCreate(str, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public static string UriToDomain(this Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            var domainParser = new DomainParser(new WebTldRuleProvider());
            return domainParser.IsValidDomain(uri.OriginalString) ? domainParser.Parse(uri).Domain : "";
        }

        public static string AddressToDomain(this string address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            return new Uri(address).UriToDomain();
        }

        public static string Take(this string str, int n)
        {
            return new string(str?.AsEnumerable().Take(n).ToArray());
        }

        public static string Take(this string str, uint u) => str.Take((int)u);
        public static string Take(this string str, long l) => str.Take((int)l);

        public static string Skip(this string str, int n)
        {
            return new string(str?.AsEnumerable().Skip(n).ToArray());
        }

        public static string SkipLastWhile(this string str, Func<char, bool> condition)
        {
            return new string(str.AsEnumerable().SkipLastWhile(condition).ToArray());
        }

        public static string TakeLast(this string str, int n)
        {
            return new string(Enumerable.TakeLast(str.AsEnumerable() ?? Enumerable.Empty<char>(), n).ToArray());
        }

        public static string SkipLast(this string str, int n)
        {
            return new string(Enumerable.SkipLast(str.AsEnumerable() ?? Enumerable.Empty<char>(), n).ToArray());
        }

        public static string First(this string str) => str.Take(1);
        public static string Last(this string str) => str.TakeLast(1);

        public static string RemoveAt(this string str, params int[] positions)
        {
            var charsList = str.AsEnumerable().ToList();
            for (var i = charsList.Count - 1; i >= 0; i--)
                if (i.EqualsAny(positions))
                    charsList.RemoveAt(i);
            return new string(charsList.ToArray());
        }

        public static string RegexReplace(this string str, string pattern, string replacement)
        {
            return Regex.Replace(str, pattern, replacement);
        }

        public static string ReplaceLast(this string str, string fromStr, string toStr)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (fromStr == null)
                throw new ArgumentNullException(nameof(fromStr));

            var lastIndexOf = str.LastIndexOf(fromStr, StringComparison.Ordinal);
            if (lastIndexOf < 0)
                return str;

            var leading = str.Substring(0, lastIndexOf);
            var charsToEnd = str.Length - (lastIndexOf + fromStr.Length);
            var trailing = str.Substring(lastIndexOf + fromStr.Length, charsToEnd);

            return leading + toStr + trailing;
        }

        public static string RemoveLast(this string str, string fromStr)
        {
            return str.ReplaceLast(fromStr, string.Empty);
        }

        public static string ReplaceMany(this string str, IEnumerable<string> strEn, string replacement)
        {
            return strEn.Aggregate(str, (current, s) => current.ReplaceInvariant(s, replacement));
        }

        public static string TrimEnd(this string str, string end)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (end == null)
                throw new ArgumentNullException(nameof(end));

            return str.EndsWithInvariant(end) ? str.Substring(0, str.Length - end.Length) : str;
        }

        public static bool ContainsOnlyDigits(this string str) => str.All(c => c is >= '0' and <= '9');

        public static bool ContainsLetters(this string str) => str.Any(c => !char.IsLetter(c));

        public static bool EndsWithDigit(this string str) => str?.AsEnumerable().Last() >= '0' && str?.AsEnumerable().Last() <= '9';

        public static string After(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, NoValueType.Throw,
                CaseType.MaintainCase, BeforeAfterInternalForMethodType.After);
        }

        public static string AfterIgnoreCase(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, NoValueType.Throw,
                CaseType.IgnoreCase, BeforeAfterInternalForMethodType.After);
        }

        public static string AfterOrNull(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, NoValueType.Null,
                CaseType.MaintainCase, BeforeAfterInternalForMethodType.After);
        }

        public static string AfterOrNullIgnoreCase(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, NoValueType.Null,
                CaseType.IgnoreCase, BeforeAfterInternalForMethodType.After);
        }

        public static string AfterOrWhole(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, NoValueType.Whole,
                CaseType.MaintainCase, BeforeAfterInternalForMethodType.After);
        }

        public static string AfterOrWholeIgnoreCase(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, NoValueType.Whole,
                CaseType.IgnoreCase, BeforeAfterInternalForMethodType.After);
        }

        public static string AfterN(this string str, string substring, int occurance = 1) => str.AfterOrNull(substring, occurance);
        public static string AfterIgnoreCaseN(this string str, string substring, int occurance = 1) => str.AfterOrNullIgnoreCase(substring, occurance);

        public static string AfterFirst(this string s, string substring)
        {
            return s.After(substring);
        }

        public static string AfterFirstIgnoreCase(this string s, string substring)
        {
            return s.AfterIgnoreCase(substring);
        }

        public static string AfterFirstOrNull(this string s, string substring)
        {
            return s.AfterOrNull(substring);
        }

        public static string AfterFirstN(this string s, string substring)
        {
            return s.AfterFirstOrNull(substring);
        }

        public static string AfterFirstOrNullIgnoreCase(this string s, string substring)
        {
            return s.AfterOrNullIgnoreCase(substring);
        }

        public static string AfterFirstIgnoreCaseN(this string s, string substring)
        {
            return s.AfterFirstOrNullIgnoreCase(substring);
        }

        public static string AfterFirstOrWhole(this string s, string substring)
        {
            return s.AfterOrWhole(substring);
        }

        public static string AfterFirstOrWholeIgnoreCase(this string s, string substring)
        {
            return s.AfterOrWholeIgnoreCase(substring);
        }

        public static string AfterLast(this string s, string substring)
        {
            return s.After(substring, -1);
        }

        public static string AfterLastIgnoreCase(this string s, string substring)
        {
            return s.AfterIgnoreCase(substring, -1);
        }

        public static string AfterLastOrNull(this string s, string substring)
        {
            return s.AfterOrNull(substring, -1);
        }

        public static string AfterLastOrNullIgnoreCase(this string s, string substring)
        {
            return s.AfterOrNullIgnoreCase(substring, -1);
        }

        public static string AfterLastN(this string s, string substring) => s.AfterLastOrNull(substring);
        public static string AfterLastIgnoreCaseN(this string s, string substring) => s.AfterLastOrNullIgnoreCase(substring);

        public static string AfterLastOrWhole(this string s, string substring)
        {
            return s.AfterOrWhole(substring, -1);
        }

        public static string AfterLastOrWholeIgnoreCase(this string s, string substring)
        {
            return s.AfterOrWholeIgnoreCase(substring, -1);
        }

        public static string Before(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, NoValueType.Throw,
                CaseType.MaintainCase, BeforeAfterInternalForMethodType.Before);
        }

        public static string BeforeIgnoreCase(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, NoValueType.Throw,
                CaseType.IgnoreCase, BeforeAfterInternalForMethodType.Before);
        }

        public static string BeforeOrNull(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, NoValueType.Null,
                CaseType.MaintainCase, BeforeAfterInternalForMethodType.Before);
        }

        public static string BeforeOrNullIgnoreCase(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, NoValueType.Null,
                CaseType.IgnoreCase, BeforeAfterInternalForMethodType.Before);
        }

        public static string BeforeOrWhole(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, NoValueType.Whole,
                CaseType.MaintainCase, BeforeAfterInternalForMethodType.Before);
        }

        public static string BeforeOrWholeIgnoreCase(this string str, string substring, int occurance = 1)
        {
            return str.BeforeAfterInternal(substring, occurance, NoValueType.Whole,
                CaseType.IgnoreCase, BeforeAfterInternalForMethodType.Before);
        }

        public static string BeforeN(this string str, string substring, int occurance = 1) => str.BeforeOrNull(substring, occurance);
        public static string BeforeIgnoreCaseN(this string str, string substring, int occurance = 1) => str.BeforeOrNullIgnoreCase(substring, occurance);

        public static string BeforeFirst(this string s, string substring)
        {
            return s.Before(substring);
        }

        public static string BeforeFirstIgnoreCase(this string s, string substring)
        {
            return s.BeforeIgnoreCase(substring);
        }

        public static string BeforeFirstOrNull(this string s, string substring)
        {
            return s.BeforeOrNull(substring);
        }

        public static string BeforeFirstN(this string s, string substring)
        {
            return s.BeforeFirstOrNull(substring);
        }

        public static string BeforeFirstOrNullIgnoreCase(this string s, string substring)
        {
            return s.BeforeOrNullIgnoreCase(substring);
        }

        public static string BeforeFirstIgnoreCaseN(this string s, string substring)
        {
            return s.BeforeFirstOrNullIgnoreCase(substring);
        }

        public static string BeforeFirstOrWhole(this string s, string substring)
        {
            return s.BeforeOrWhole(substring);
        }

        public static string BeforeFirstOrWholeIgnoreCase(this string s, string substring)
        {
            return s.BeforeOrWholeIgnoreCase(substring);
        }

        public static string BeforeLast(this string s, string substring)
        {
            return s.Before(substring, -1);
        }

        public static string BeforeLastIgnoreCase(this string s, string substring)
        {
            return s.BeforeIgnoreCase(substring, -1);
        }

        public static string BeforeLastOrNull(this string s, string substring)
        {
            return s.BeforeOrNull(substring, -1);
        }

        public static string BeforeLastOrNullIgnoreCase(this string s, string substring)
        {
            return s.BeforeOrNullIgnoreCase(substring, -1);
        }

        public static string BeforeLastN(this string s, string substring) => s.BeforeLastOrNull(substring);
        public static string BeforeLastIgnoreCaseN(this string s, string substring) => s.BeforeLastOrNullIgnoreCase(substring);

        public static string BeforeLastOrWhole(this string s, string substring)
        {
            return s.BeforeOrWhole(substring, -1);
        }

        public static string BeforeLastOrWholeIgnoreCase(this string s, string substring)
        {
            return s.BeforeOrWholeIgnoreCase(substring, -1);
        }

        private static string BeforeAfterInternal(this string str, string substring, int occurance, NoValueType nullValue, CaseType casing, BeforeAfterInternalForMethodType methodType)
        {
            if (occurance == 0)
                throw new ArgumentOutOfRangeException(nameof(occurance));

            if (str.IsNullOrEmpty())
            {
                return nullValue switch
                {
                    NoValueType.Null => null,
                    NoValueType.Whole => str,
                    NoValueType.Throw => throw new ArgumentNullException(nameof(str)),
                    _ => throw new ArgumentNullException(nameof(str))
                };
            }

            if (substring.IsNullOrEmpty())
            {
                return nullValue switch
                {
                    NoValueType.Null => null,
                    NoValueType.Whole => str,
                    NoValueType.Throw => throw new ArgumentNullException(nameof(substring)),
                    _ => throw new ArgumentNullException(nameof(substring))
                };
            }

            if (casing == CaseType.IgnoreCase ? !str.ContainsIgnoreCase(substring) : !str.ContainsInvariant(substring))
            {
                return nullValue switch
                {
                    NoValueType.Null => null,
                    NoValueType.Whole => str,
                    NoValueType.Throw => throw new Exception("String doesn't contain substring"),
                    _ => throw new Exception("String doesn't contain substring")
                };
            }

            if (casing == CaseType.IgnoreCase ? str.EqualsIgnoreCase(substring) : str.EqualsInvariant(substring))
            {
                return nullValue switch
                {
                    NoValueType.Null => null,
                    NoValueType.Whole => str,
                    NoValueType.Throw => throw new Exception("String equals substring"),
                    _ => throw new Exception("String equals substring")
                };
            }
            var split = casing == CaseType.IgnoreCase ? str.SplitIgnoreCase(substring) : str.Split(substring);
            var separators = str.SeparatorsOrNullIgnoreCase(substring);

            var result = casing switch
            {
                CaseType.IgnoreCase => methodType switch
                {
                    BeforeAfterInternalForMethodType.After => split[(occurance < 0 ? ^-occurance : occurance)..].JoinAsString(separators[(occurance < 0 ? ^-(occurance + 1) : occurance)..]),
                    BeforeAfterInternalForMethodType.Before => split[..(occurance < 0 ? ^-occurance : occurance)].JoinAsString(separators[..(occurance < 0 ? ^-(occurance + 1) : occurance)]),
                    _ => throw new ArgumentOutOfRangeException(nameof(methodType))
                },
                CaseType.MaintainCase => methodType switch
                {
                    BeforeAfterInternalForMethodType.After => split[(occurance < 0 ? ^-occurance : occurance)..].JoinAsString(substring),
                    BeforeAfterInternalForMethodType.Before => split[..(occurance < 0 ? ^-occurance : occurance)].JoinAsString(substring),
                    _ => throw new ArgumentOutOfRangeException(nameof(methodType))
                },
                _ => throw new ArgumentOutOfRangeException(nameof(casing))
            };

            return nullValue switch
            {
                NoValueType.Null => result,
                NoValueType.Whole => result ?? str,
                NoValueType.Throw => result ?? throw new NullReferenceException(nameof(result)),
                _ => throw new NullReferenceException(nameof(result))
            };
        }

        private enum NoValueType
        {
            Null,
            Whole,
            Throw
        }

        private enum CaseType
        {
            MaintainCase,
            IgnoreCase
        }

        private enum BeforeAfterInternalForMethodType
        {
            Before,
            After
        }

        public static bool IsBefore(this string str, string word1, string word2)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            var idx1 = str.IndexOf(word1, StringComparison.Ordinal);
            var idx2 = str.IndexOf(word2, StringComparison.Ordinal);

            if (idx1 < 0 || idx2 < 0 || word1 == word2)
                throw new Exception("Words are invalid");
            return idx1 <= idx2;
        }

        public static bool IsAfter(this string str, string word1, string word2)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            var idx1 = str.IndexOf(word1, StringComparison.Ordinal);
            var idx2 = str.IndexOf(word2, StringComparison.Ordinal);

            if (idx1 < 0 || idx2 < 0 || word1 == word2)
                throw new Exception("Words are invalid");
            return idx1 > idx2;
        }

        public static string[] SeparatorsOrNull(this string s, string substring)
        {
            var matches = Regex.Matches(s, Regex.Escape(substring), RegexOptions.CultureInvariant).Select(m => m.Value).ToArray();
            return matches.Any() ? matches : null;
        }

        public static string[] SeparatorsOrNullIgnoreCase(this string s, string substring)
        {
            var matches = Regex.Matches(s, Regex.Escape(substring), RegexOptions.CultureInvariant | RegexOptions.IgnoreCase).Select(m => m.Value).ToArray();
            return matches.Any() ? matches : null;
        }

        public static bool EqualsInvariant(this string str1, string str2)
        {
            if (str1 is null && str2 is null)
                return true;
            if (str1 is null || str2 is null)
                return false;

            return str1.Equals(str2, StringComparison.InvariantCulture);
        }

        public static string RemoveHexPrefix(this string value)
        {
            return value.StartsWithInvariant("0x") ? value.Skip(2) : value;
        }

        public static string EnforceHexPrefix(this string str)
        {
            return str.StartsWithInvariant("0x") ? str : $"0x{str}";
        }

        public static bool IsNumber(this string str)
        {
            return str.All(char.IsDigit);
        }

        public static string SkipWhile(this string str, Func<char, bool> condition)
        {
            return new string(str.AsEnumerable().SkipWhile(condition).ToArray());
        }

        public static string TakeWhile(this string str, Func<char, bool> condition)
        {
            return new string(str.AsEnumerable().TakeWhile(condition).ToArray());
        }

        public static string SkipWhileDigit(this string str)
        {
            return new string(str.AsEnumerable().SkipWhile(char.IsDigit).ToArray());
        }

        public static string TakeWhileDigit(this string str)
        {
            return new string(str.AsEnumerable().TakeWhile(char.IsDigit).ToArray());
        }

        public static string TrimMultiline(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return str.Split(Environment.NewLine).Select(line => line.Trim()).JoinAsString().Trim();
        }

        public static HtmlDocument HTML(this string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }

        public static bool IsHTML(this string strHtml)
        {
            var docHtml = strHtml.HTML();
            return !docHtml.ParseErrors.Any() && docHtml.Root().Descendants().Any(n => n.NodeType != HtmlNodeType.Text);
        }

        public static bool EqualsIgnoreCase(this string str, string ostr)
        {
            return string.Equals(str, ostr, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool EqAnyIgnoreCase(this string str, params string[] os)
        {
            return os.Any(s => s.EqualsIgnoreCase(str));
        }

        public static bool EqAnyIgnoreCase(this string str, IEnumerable<string> os)
        {
            return os.Any(s => s.EqualsIgnoreCase(str));
        }

        public static string EnsureSuffix(this string str, string suffix)
        {
            if (!str.EndsWithInvariant(suffix))
                str += suffix;
            return str;
        }

        public static bool ContainsAny(this string str, IEnumerable<string> strings)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            var lStr = str.ToUpperInvariant();
            return strings.Select(s => s.ToUpperInvariant()).Any(lStr.Contains);
        }

        public static bool ContainsAll(this string str, IEnumerable<string> strings)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            var lStr = str.ToUpperInvariant();
            return strings.Select(s => s.ToUpperInvariant()).All(lStr.Contains);
        }

        public static string Repeat(this string str, int n)
        {
            return new string(str.AsEnumerable().Repeat(n).ToArray());
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string RemoveWhiteSpace(this string str)
        {
            return str.RemoveMany("\r\n", " ");
        }

        public static bool IsIP(this string str)
        {
            return
                IPAddress.TryParse(str, out var address) &&
                address.AddressFamily.In(AddressFamily.InterNetwork, AddressFamily.InterNetworkV6);
        }

        public static bool IsBase58(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return str.All(c => PszBase58.ContainsInvariant(c));
        }

        public static string AddSpacesToPascalCase(this string text, bool wordsToLower = false, bool preserveAcronyms = true)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            var sb = new StringBuilder(text.Length * 2);
            sb.Append(text[0]);
            for (var i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if (text[i - 1] != ' ' && !char.IsUpper(text[i - 1]) || preserveAcronyms && char.IsUpper(text[i - 1]) && i < text.Length - 1 && !char.IsUpper(text[i + 1]))
                        sb.Append(' ');
                sb.Append(wordsToLower ? text[i].ToStringInvariant().ToLowerInvariant() : text[i].ToStringInvariant());
            }
            return sb.ToString();
        }

        public static string Shuffle(this string str)
        {
            return new string(str.AsEnumerable().Shuffle().ToArray());
        }

        public static bool IsJson(this string json)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));

            json = json.Trim();
            if (json.StartsWithInvariant("{") && json.EndsWithInvariant("}") ||
                json.StartsWithInvariant("[") && json.EndsWithInvariant("]"))
            {
                try
                {
                    JToken.Parse(json);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    Console.WriteLine(jex.Message);
                    return false;
                }
            }

            return false;
        }

        public static string RemoveScientificNotation(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            var x = str.IndexOf_('E');
            if (x < 0) return str;

            var x1 = x + 1;
            var exp = str[x1..];
            var e = int.Parse(exp, NumberStyles.Any, CultureInfo.InvariantCulture);

            string s;
            var numDecimals = 0;
            if (str.StartsWithInvariant("-"))
            {
                var len = x - 3;
                if (e >= 0)
                {
                    if (len > 0)
                    {
                        s = str.Substring(0, 2) + str.Substring(3, len);
                        numDecimals = len;
                    }
                    else
                        s = str.Substring(0, 2);
                }
                else
                {
                    if (len > 0)
                    {
                        s = str.Substring(1, 1) + str.Substring(3, len);
                        numDecimals = len;
                    }
                    else
                        s = str.Substring(1, 1);
                }
            }
            else
            {
                var len = x - 2;
                if (len > 0)
                {
                    s = str[0] + str.Substring(2, len);
                    numDecimals = len;
                }
                else
                    s = str[0].ToStringInvariant();
            }

            if (e >= 0)
            {
                e -= numDecimals;
                var z = Enumerable.Repeat("0", e).JoinAsString();
                s += z;
            }
            else
            {
                e = -e - 1;
                var z = Enumerable.Repeat("0", e).JoinAsString();
                if (str.StartsWithInvariant("-"))
                    s = "-0." + z + s;
                else
                    s = "0." + z + s;
            }

            return s;
        }

        public static bool EndsWithIgnoreCase(this string str, string tail)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (tail == null)
                throw new ArgumentNullException(nameof(tail));

            return str.ToLowerInvariant().EndsWithInvariant(tail.ToLowerInvariant());
        }

        public static bool StartsWithIgnoreCase(this string str, string head)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (head == null)
                throw new ArgumentNullException(nameof(head));

            return str.ToLowerInvariant().StartsWithInvariant(head.ToLowerInvariant());
        }

        public static string StartWithUpper(this string str) => str.Take(1).ToUpperInvariant() + str.Skip(1);

        public static bool IsUpper(this string str) => !str.Any(char.IsLower);

        public static bool IsLower(this string str) => !str.Any(char.IsUpper);

        public static int IndexOfInvariant(this string s, string substring)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return s.IndexOf(substring, StringComparison.InvariantCulture);
        }

        public static int IndexOfIgnoreCase(this string s, string substring)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return s.IndexOf(substring, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this string s, string substring)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            return s.Contains(substring, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string Prepend(this string s, string pre) => pre + s;
        public static string Append(this string s, string app) => s + app;
        public static string VerboseNull(this string s) => s.IsNullOrWhiteSpace() ? "null" : s;
        public static string ReplacePercentagesWithValues(this string s)
        {
            if (s == null)
                throw new NullReferenceException(nameof(s));

            const string precNumsRx = @"[^\S\r\n]*(?:\d+(?:\.\d*)?|\.\d+)%[^\S\r\n]*";
            var matches = Regex.Matches(s, precNumsRx).SelectMany(o => o.Captures).ToArray();
            foreach (var match in matches)
                s = s.Remove(match.Index, match.Length).Insert(match.Index, $"{match.Value.BeforeFirst("%")} / 100 * ");
            return s;
        }

        public static string ReplaceFirst(this string text, string search, string replace)
        {
            if (search == null)
                throw new NullReferenceException(nameof(search));

            var pos = text.IndexOfInvariant(search);
            if (pos < 0)
                return text;

            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string ReplaceAt(this string s, int at, int sourceSubstringLength, string with)
        {
            if (s == null)
                throw new NullReferenceException(nameof(s));

            return s.Remove(at, sourceSubstringLength).Insert(at, with);
        }

        public static TimeSpan TimeFrameToTimeSpan(this string timeFrame)
        {
            if (timeFrame.EqualsIgnoreCase("1w"))
                return TimeSpan.FromDays(7);
            if (timeFrame.EqualsIgnoreCase("1d"))
                return TimeSpan.FromDays(1);
            if (timeFrame.EqualsIgnoreCase("12h"))
                return TimeSpan.FromHours(12);
            if (timeFrame.EqualsIgnoreCase("6h"))
                return TimeSpan.FromHours(6);
            if (timeFrame.EqualsIgnoreCase("4h"))
                return TimeSpan.FromHours(4);
            if (timeFrame.EqualsIgnoreCase("3h"))
                return TimeSpan.FromHours(3);
            if (timeFrame.EqualsIgnoreCase("2h"))
                return TimeSpan.FromHours(2);
            if (timeFrame.EqualsIgnoreCase("1h"))
                return TimeSpan.FromHours(1);
            if (timeFrame.EqualsIgnoreCase("30m"))
                return TimeSpan.FromMinutes(30);
            if (timeFrame.EqualsIgnoreCase("15m"))
                return TimeSpan.FromMinutes(15);
            if (timeFrame.EqualsIgnoreCase("5m"))
                return TimeSpan.FromMinutes(5);
            if (timeFrame.EqualsIgnoreCase("1m"))
                return TimeSpan.FromMinutes(1);
            throw new ArgumentOutOfRangeException(nameof(timeFrame));
        }

        public static bool ContainsWord(this string s, string word)
        {
            if (s == null)
                throw new NullReferenceException(nameof(s));

            return s.Split(" ").Any(w => w.EqualsInvariant(word));
        }

        public static bool ContainsWordIgnoreCase(this string s, string word)
        {
            if (s == null)
                throw new NullReferenceException(nameof(s));

            return s.Split(" ").Any(w => w.EqualsIgnoreCase(word));
        }

        public static string ReplaceWord(this string s, string word, string with)
        {
            if (s == null)
                throw new NullReferenceException(nameof(s));

            return s.Split(" ").Select(w => w.EqualsInvariant(word) ? with : w).JoinAsString(" ");
        }

        public static string ReplaceWordIgnoreCase(this string s, string word, string with)
        {
            if (s == null)
                throw new NullReferenceException(nameof(s));

            return s.Split(" ").Select(w => w.EqualsIgnoreCase(word) ? with : w).JoinAsString(" ");
        }

        private static string IntersectInternal(this string s, string sub, NoValueType noValueType, CaseType casing)
        {
            var i = -1;
            if (!s.IsNullOrEmpty() && !sub.IsNullOrEmpty())
            {
                i = s.IndexOfInvariant(sub);
                if (i < 0)
                {
                    i = sub.IndexOfInvariant(s);
                    if (i >= 0)
                        (s, sub) = (sub, s);
                }
            }

            if (s.IsNullOrEmpty() || sub.IsNullOrEmpty() || i < 0)
            {
                return noValueType switch
                {
                    NoValueType.Null => null,
                    NoValueType.Whole => s,
                    NoValueType.Throw => s.IsNullOrEmpty() 
                        ? throw new ArgumentNullException(nameof(s))
                        : sub.IsNullOrEmpty()
                            ? throw new ArgumentNullException(nameof(s)) 
                            : i < 0
                                ? throw new IndexOutOfRangeException($"{nameof(s)} and {nameof(sub)} don't intersect")
                                : throw new Exception("This should not have happened"),
                    _ => throw new Exception("This should not have happened"),
                };
            }
            
            var separators = casing == CaseType.IgnoreCase ? s.SeparatorsOrNullIgnoreCase(sub) : s.SeparatorsOrNull(sub);
            if (separators.Length > 1)
                throw new ArgumentOutOfRangeException(null, "String contains more than one occurance of substring");

            return separators[0];
        }

        public static string Intersect(this string s, string sub) => s.IntersectInternal(sub, NoValueType.Throw, CaseType.MaintainCase);
        public static string IntersectOrNull(this string s, string sub) => s.IntersectInternal(sub, NoValueType.Null, CaseType.MaintainCase);
        public static string IntersectOrWhole(this string s, string sub) => s.IntersectInternal(sub, NoValueType.Whole, CaseType.MaintainCase);
        public static string IntersectIgnoreCase(this string s, string sub) => s.IntersectInternal(sub, NoValueType.Throw, CaseType.IgnoreCase);
        public static string IntersectOrNullIgnoreCase(this string s, string sub) => s.IntersectInternal(sub, NoValueType.Null, CaseType.IgnoreCase);
        public static string IntersectOrWholeIgnoreCase(this string s, string sub) => s.IntersectInternal(sub, NoValueType.Whole, CaseType.IgnoreCase);

        public static bool IsEmailAddress(this string s) => Regex.IsMatch(s, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");

        public static string NullifyIfNullOrEmpty(this string s) => s.NullifyIf(str => str.IsNullOrEmpty());
        public static string NullifyIfNullOrWhiteSpace(this string s) => s.NullifyIf(str => str.IsNullOrWhiteSpace());
    }
}
