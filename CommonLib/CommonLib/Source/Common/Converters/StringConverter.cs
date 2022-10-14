using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Converters
{
    public static class StringConverter
    {
        public static string ToStringInvariant(this object o)
        {
            return Convert.ToString(o, CultureInfo.InvariantCulture);
        }

        public static string ToStringN(this object o)
        {
            if (o == null)
                return null;
            string strO = null;
            
            try { strO = o.ToString(); } // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception) { }
            return string.IsNullOrWhiteSpace(strO) ? null : strO;
        }

        public static T Parse<T>(this string str, Func<string, T> parser)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            return parser(str);
        }

        public static async Task<T> Parse<T>(this Task<string> taskStr, Func<string, T> parser)
        {
            if (taskStr == null)
                throw new ArgumentNullException(nameof(taskStr));

            return (await taskStr.ConfigureAwait(false)).Parse(parser);
        }

        public static string ToIP(this string domainOrIp)
        {
            var ip = domainOrIp.IsIP() ? domainOrIp : Dns.GetHostAddresses(domainOrIp)[0].ToString();
            return ip.In("::1", "0.0.0.0") ? "127.0.0.1" : ip;
        }

        public static string KebabCaseToPascalCase(this string s)
        {
            if (s == null)
                return null;

            var startsWithUnderScore = s.StartsWithInvariant("_");
            var words = s.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder(words.Sum(x => x.Length));

            foreach (var word in words)
                sb.Append(string.Concat(word[0].ToStringInvariant().ToUpperInvariant(), word.AsSpan(1)));
            if (startsWithUnderScore)
                sb.Prepend('_');

            return sb.ToString();
        }

        public static string PascalCaseToKebabCase(this string source)
        {
            if (source is null) return null;
            if (source.Length == 0) return string.Empty;

            var sb = new StringBuilder();

            for (var i = 0; i < source.Length; i++)
            {
                if (char.IsLower(source[i]) || char.IsPunctuation(source[i])) // if current char is already lowercase or puntuation like `/`
                    sb.Append(source[i]);
                else if (i == 0 || char.IsPunctuation(source[i - 1])) // if current char is the first char or punctuation like `/`
                    sb.Append(source[i].ToLowerInvariant());
                else if (char.IsLower(source[i - 1])) // if current char is upper and previous char is lower
                {
                    sb.Append('-');
                    sb.Append(source[i].ToLowerInvariant());
                }
                else if (i + 1 == source.Length || char.IsUpper(source[i + 1])) // if current char is upper and next char doesn't exist or is upper
                    sb.Append(source[i].ToLowerInvariant());
                else // if current char is upper and next char is lower
                {
                    sb.Append('-');
                    sb.Append(source[i].ToLowerInvariant());
                }
            }
            return sb.ToString();
        }

        public static string PascalCaseToCamelCase(this string s)
        {
            return s.Take(1).ToLowerInvariant() + s.Skip(1);
        }
    }
}
