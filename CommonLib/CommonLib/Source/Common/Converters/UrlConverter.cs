using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using CommonLib.Source.Common.Extensions;
using CommonLib.Source.Common.Extensions.Collections;
using MoreLinq.Extensions;
using Truncon.Collections;
using WebSocketSharp;

namespace CommonLib.Source.Common.Converters
{
    public static class UrlConverter
    {
        public static Dictionary<string, string> QueryStringToDictionary(this string queryString)
        {
            var qsp = queryString.AfterFirstOrNullIgnoreCase("?");
            return qsp == null ? new Dictionary<string, string>() : HttpUtility.ParseQueryString(qsp).AsEnumerable().ToDictionary();
        }

        //public static Dictionary<string, string> QueryStringToDictionary(this string queryString)
        //{
        //    var nvc = HttpUtility.ParseQueryString(queryString);
        //    return nvc.AsEnumerable().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        //}

        public static string ToQueryString(this IDictionary<string, string> parameters)
        {
            if (parameters is not { Count: > 0 }) return "";
            var sb = new StringBuilder();
            foreach (var (name, value) in parameters)
                sb.Append($"&{name.ToAddressEncoded()}={value.ToAddressEncoded()}");
            return sb.ToString().Skip(1);
        }

        public static string ToQueryString(this OrderedDictionary<string, string> parameters)
        {
            if (parameters is not { Count: > 0 }) return "";
            var sb = new StringBuilder();
            foreach (var (name, value) in parameters)
                if (!value.IsNullOrWhiteSpace())
                    sb.Append($"&{name.ToAddressEncoded()}={value.ToAddressEncoded()}");
            return sb.ToString().Skip(1);
        }

        public static string ToQueryString(this IEnumerable<KeyValuePair<string, string>> parameters) => parameters.ToOrderedDictionary().ToQueryString();

        public static string ToAddressEncoded(this string str) => Uri.EscapeDataString(str); // Uri.EscapeDataString HttpUtility.UrlPathEncode

        public static string HtmlEncode(this string s)
        {
            var sb = new StringBuilder();
            var bytes = s.UTF8ToByteArray();
            foreach (var b in bytes)
            {
                if (b >= 0x41 && b <=0x5A || b >= 0x61 && b <=0x7A || b >= 0x30 && b <=0x39 || b == '-' || b == '.' || b == '_' || b == '~')
                    sb.Append((char) b);
                else
                    sb.Append($"%{Convert.ToString(b, 16).ToUpper()}");
            }
            return sb.ToString();
        }

        public static string HtmlDecode(this string s)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < s.Length; i++)
            {
                var curr = s[i].ToString();
                if (s[i] == '%')
                {
                    curr = $"{s[i + 1]}{s[i + 2]}";
                    sb.Append((char) Convert.ToByte(curr, 16));
                    i += 2;
                }
                else
                    sb.Append(curr);
            }

            return sb.ToString();
        }

        public static Uri ToUri(this string value)
        {
            Uri.TryCreate(value, value.IsUri() ? UriKind.Absolute : UriKind.Relative, out var result);
            return result;
        }
    }
}
