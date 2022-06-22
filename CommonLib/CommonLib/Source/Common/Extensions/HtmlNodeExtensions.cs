using System;
using HtmlAgilityPack;

namespace CommonLib.Source.Common.Extensions
{
    public static class HtmlNodeExtensions
    {
        public static string GetAttributeValue(this HtmlNode node, string attr)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.GetAttributeValue(attr, null);
        }

        public static string Attribute(this HtmlNode node, string attr) => node.GetAttributeValue(attr);
    }
}
