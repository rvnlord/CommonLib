using System;
using System.Globalization;
using System.Linq;
using CommonLib.Source.Common.Utils.TypeUtils;
using Org.BouncyCastle.Math;

namespace CommonLib.Source.Common.Converters
{
    public static class HexConverter
    {
        public static string ToHexString(this BigInteger number)
        {
            if (number == null)
                throw new ArgumentNullException(nameof(number));

            return number.ToString(16);
        }

        public static string ToHexString(this int n) => n.ToStringInvariant("X");

        public static string HexToString(this byte[] value, bool prefix = false)
        {
            var strPrex = prefix ? "0x" : "";
            return strPrex + string.Concat(value.Select(b => b.ToStringInvariant("x2")).ToArray());
        }

        public static byte[] HexToByteArray(this string str)
        {
            byte[] bytes;
            if (string.IsNullOrEmpty(str))
                bytes = Array.Empty<byte>();
            else
            {
                var string_length = str.Length;
                var character_index = str.StartsWith("0x", StringComparison.Ordinal) ? 2 : 0;
                var number_of_characters = string_length - character_index;
                var add_leading_zero = false;

                if (0 != number_of_characters % 2)
                {
                    add_leading_zero = true;
                    number_of_characters += 1;
                }

                bytes = new byte[number_of_characters / 2];

                var write_index = 0;
                if (add_leading_zero)
                {
                    bytes[write_index++] = CharUtils.CharacterToByte(str[character_index], character_index);
                    character_index += 1;
                }

                for (var read_index = character_index; read_index < str.Length; read_index += 2)
                {
                    var upper = CharUtils.CharacterToByte(str[read_index], read_index, 4);
                    var lower = CharUtils.CharacterToByte(str[read_index + 1], read_index + 1);

                    bytes[write_index++] = (byte)(upper | lower);
                }
            }

            return bytes;
        }

        public static int HexToInt(this string str)
        {
            return int.Parse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        public static string ToHexString(this byte[] value, bool prefix = false)
        {
            var strPrex = prefix ? "0x" : "";
            return strPrex + string.Concat(value.Select(b => b.ToStringInvariant("x2")).ToArray());
        }
    }
}
