using System;
using HtmlAgilityPack;

namespace CommonLib.Source.Common.Extensions
{
    public static class HtmlDocumentExtensions
    {
        public static HtmlNode Root(this HtmlDocument html)
        {
            if (html == null)
                throw new ArgumentNullException(nameof(html));

            return html.DocumentNode;
        }
    }
}
