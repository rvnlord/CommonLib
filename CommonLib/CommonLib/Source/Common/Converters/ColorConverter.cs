using System;
using CommonLib.Source.Common.Extensions;
using SixLabors.ImageSharp;

namespace CommonLib.Source.Common.Converters
{
    public static class ColorConverter
    {
        public static Color FromHex(string hexColor)
        {
            if (hexColor.IsNullOrWhiteSpace())
                throw new NullReferenceException("hex color is empty");
            hexColor = hexColor.RemoveHexPrefix().RemoveHashPrefix();
            if (!hexColor.IsHex() || !hexColor.Length.In(6, 8))
                throw new FormatException("hex color has invalid format");

            var r = hexColor.Take(2).HexToByte();
            var g = hexColor.Skip(2).Take(2).HexToByte();
            var b = hexColor.Skip(4).Take(2).HexToByte();
            var a = hexColor.Length == 8 ? hexColor.Skip(6).Take(2).HexToByte() : (byte) 255;

            return Color.FromRgba(r, g, b, a);
        }

        public static Color HexToColor(this string hexColor) =>FromHex(hexColor);
    }
}
