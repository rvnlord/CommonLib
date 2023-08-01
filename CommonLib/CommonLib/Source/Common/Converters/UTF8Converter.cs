using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib.Source.Common.Converters
{
    public static class UTF8Converter
    {
        public static byte[] UTF8ToByteArray(this string str) => Encoding.UTF8.GetBytes(str);
        public static string UTF8ToString(this byte[] arr) => Encoding.UTF8.GetString(arr);
        public static byte[] HexToUTF8(this byte[] arr) => arr.ToHexString().UTF8ToByteArray();
        public static byte[] UTF8ToHex(this byte[] arr) => arr.UTF8ToString().HexToByteArray();
        public static byte[] HexToUTF8(this IEnumerable<byte> en) => en.ToArray().HexToUTF8();
        public static byte[] UTF8ToHex(this IEnumerable<byte> en) => en.ToArray().UTF8ToHex();
        public static string UTF8ToHex(this string strUtf8) => strUtf8.UTF8ToByteArray().HexToString();
        public static string HexToUTF8(this string strHex) => strHex.HexToByteArray().UTF8ToString();
    }
}
