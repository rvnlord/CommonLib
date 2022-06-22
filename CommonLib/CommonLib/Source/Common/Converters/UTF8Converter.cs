using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib.Source.Common.Converters
{
    public static class UTF8Converter
    {
        public static byte[] UTF8ToByteArray(this string str) => Encoding.UTF8.GetBytes(str);
        public static string ToUTF8String(this byte[] arr) => Encoding.UTF8.GetString(arr);
        public static byte[] HexToUTF8(this byte[] arr) => arr.ToHexString().UTF8ToByteArray();
        public static byte[] UTF8ToHex(this byte[] arr) => arr.ToUTF8String().HexToByteArray();
        public static byte[] HexToUTF8(this IEnumerable<byte> en) => en.ToArray().HexToUTF8();
        public static byte[] UTF8ToHex(this IEnumerable<byte> en) => en.ToArray().UTF8ToHex();
    }
}
