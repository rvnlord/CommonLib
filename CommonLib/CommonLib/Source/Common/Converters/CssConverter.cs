using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommonLib.Source.Common.Extensions;
using CommonLib.Source.Common.Extensions.Collections;
using ExCSS;
using Truncon.Collections;

namespace CommonLib.Source.Common.Converters
{
    public static class CssConverter
    {
        public static Dictionary<string, string> CssStringToDictionary(this string css)
        {
            return css.IsNullOrWhiteSpace() 
                ? new Dictionary<string, string>() 
                : css.Split(";").Where(r => !r.IsNullOrWhiteSpace()).ToDictionary(s => s.Split(":")[0].Trim(), s => s.Split(":")[1].Trim());
        }

        public static string CssDictionaryToString(this Dictionary<string, string> css)
        {
            return css.Select(kvp => kvp.Key + ": " + kvp.Value).JoinAsString("; ");
        }

        public static string CssDictionaryToString(this OrderedDictionary<string, string> css)
        {
            return css.Select(kvp => kvp.Key + ": " + kvp.Value).JoinAsString("; ");
        }

        public static string CssDictionaryToString(this ConcurrentDictionary<string, string> css)
        {
            return css.Select(kvp => kvp.Key + ": " + kvp.Value).JoinAsString("; ");
        }

        public static string Px(this double d) => $"{d.Round(8).ToStringInvariant()}{(d.Eq(0) ? "" : "px")}";
        public static string Px(this int i) => $"{i}{(i == 0 ? "" : "px")}";    

        public static string Rem(this double d) => $"{d.Round(8).ToStringInvariant()}{(d.Eq(0) ? "" : "rem")}";

        public static string Rem(this int i) => $"{i}{(i == 0 ? "" : "rem")}";

        public static Stylesheet ToExCss(this string css) => new StylesheetParser(false, false, false, true).Parse(css);

        public static StyleDeclaration ToExCssDeclaration(this string css)
        {
            var parser = new StylesheetParser(false, false, false, true);
            var stylesheet = parser.Parse($".dummy {{ {css} }}");
            var rule = (StyleDeclaration) stylesheet.Children.First().Children.First(c => c is StyleDeclaration);
            return rule;
        }

        public static string ExCssToString(this StylesheetNode css, IStyleFormatter formatter = null)
        {
            if (css == null)
                throw new ArgumentNullException(nameof(css));

            using var sw = new StringWriter();
            css.ToCss(sw, formatter ?? new CompressedStyleFormatter()); // ReadableStyleFormatter was deprecated?
            return sw.ToString();
        }

        public static string ExCssToSingleLineString(this StylesheetNode css) => css.ExCssToString(new CompressedStyleFormatter());
    }
}
