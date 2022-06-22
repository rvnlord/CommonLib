using System.Globalization;

namespace CommonLib.Source.Common.Converters
{
    public static class CharConverter
    {
        public static char ToLowerInvariant(this char c)
        {
            return char.ToLower(c, CultureInfo.InvariantCulture);
        }
    }
}
