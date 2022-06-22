using System;
using CommonLib.Source.Common.Extensions;
using HtmlAgilityPack;

namespace CommonLib.Source.Common.Converters
{
    public static class HtmlConverter
    {

        public static HtmlNode ToHtmlAgility(this string strHtml)
        {
            return strHtml.HTML().Root();
        }

        public static string HtmlAgilityToString(this HtmlNode html)
        {
            if (html == null)
                throw new ArgumentNullException(nameof(html));

            return html.OuterHtml;
        }
    }
}
