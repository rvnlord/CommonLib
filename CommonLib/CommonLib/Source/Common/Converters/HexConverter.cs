using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using CommonLib.Source.Common.Utils;
using CommonLib.Source.Common.Utils.TypeUtils;
using MoreLinq;
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

        public static string ToHexString(this IEnumerable<byte> value, bool prefix = false)
        {
            var strPrex = prefix ? "0x" : "";
            return strPrex + string.Concat(value.Select(b => b.ToStringInvariant("x2")).ToArray());
        }

        public static byte[] ToByteArray(this int n, Endian endian = Endian.InheritFromHardware)
        {
            //return n.ToHexString().HexToByteArray().PadStart(4).ToArray();
            //return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(n));
            endian = BitUtils.GetEndianIfInherited(endian);
            return endian != Endian.LittleEndian 
                ? new[] { (byte)(n >> 24), (byte)(n >> 16), (byte)(n >> 8), (byte)(n >> 0) }
                : new[] { (byte)(n >> 0), (byte)(n >> 8), (byte)(n >> 16), (byte)(n >> 24) };
        }

        public static byte[] ToHexByteArray(this int n) => n.ToByteArray();

        public static int ToInt(this byte[] bytes, Endian endian = Endian.InheritFromHardware)
        {
            //return (BitConverter.IsLittleEndian ? bytes : bytes.Reverse().ToArray()).HexToString().HexToInt();
            //return BitConverter.ToInt32(BitConverter.IsLittleEndian ? bytes.Reverse().ToArray() : bytes);
            bytes = bytes.EnforceLittleEndian(endian).Pad(4).ToArray();
            return (bytes[3] << 24) | ((bytes[2] & 0xff) << 16) | ((bytes[1] & 0xff) << 8) | (bytes[0] & 0xff);
        }

        public static byte[] ToByteArray(this long n, Endian endian = Endian.InheritFromHardware)
        {
            endian = BitUtils.GetEndianIfInherited(endian);
            return endian == Endian.LittleEndian
                ? BitConverter.GetBytes(n)
                : BitConverter.GetBytes(n).Reverse().ToArray();
        }
        
        public static long ToLong(this byte[] bytes, Endian endian = Endian.InheritFromHardware)
        {
            endian = BitUtils.GetEndianIfInherited(endian);
            return endian == Endian.LittleEndian
                ? BitConverter.ToInt64(bytes)
                : BitConverter.ToInt64(bytes.Reverse().ToArray());
        }
    }

    public enum Endian
    {
        LittleEndian,
        BigEndian,
        InheritFromHardware
    }
}
