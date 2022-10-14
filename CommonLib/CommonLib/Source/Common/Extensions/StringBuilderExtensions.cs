using System.Text;

namespace CommonLib.Source.Common.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder Prepend(this StringBuilder sb, string s)
        {
            sb.Insert(0, s);
            return sb;
        }

        public static StringBuilder Prepend(this StringBuilder sb, char c)
        {
            sb.Insert(0, c);
            return sb;
        }
    }
}
