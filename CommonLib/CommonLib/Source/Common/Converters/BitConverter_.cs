using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonLib.Source.Common.Extensions.Collections;
using CommonLib.Source.Common.Utils;

namespace CommonLib.Source.Common.Converters
{
    public static class BitConverter_
    {
        public static BitArray ToBitArray(this IEnumerable<bool> bits, Endian endian = Endian.InheritFromHardware) => BitUtils.GetEndianIfInherited(endian) == Endian.LittleEndian ? new BitArray(bits.ToArray()) : new BitArray(bits.Reverse().ToArray());
        public static BitArray ToBitArray(this int n, Endian endian = Endian.InheritFromHardware) => new (n.ToByteArray(endian));
        public static BitArray ToBitArray(this long n, Endian endian = Endian.InheritFromHardware) => new (n.ToByteArray(endian));
        public static BitArray ToBitArray(this byte b, Endian endian = Endian.InheritFromHardware) => BitUtils.GetEndianIfInherited(endian) == Endian.LittleEndian ? new BitArray(new[] { b }) : new BitArray(new BitArray(new[] { b }).Cast<bool>().Reverse().ToArray());
        public static BitArray ToBitArray(this IEnumerable<byte> bytes, Endian endian = Endian.InheritFromHardware) => BitUtils.GetEndianIfInherited(endian) == Endian.LittleEndian ? new BitArray(bytes.ToArray()) : new BitArray(new BitArray(bytes.ToArray()).Cast<bool>().Reverse().ToArray());

        public static T[] ToBitArray<T>(this int n, Endian endian = Endian.InheritFromHardware) => n.ToBitArray(endian).Cast<T>().ToArray();
        public static T[] ToBitArray<T>(this long n, Endian endian = Endian.InheritFromHardware) => n.ToBitArray(endian).Cast<T>().ToArray();
        public static T[] ToBitArray<T>(this byte b, Endian endian = Endian.InheritFromHardware) => b.ToBitArray(endian).Cast<T>().ToArray();
        public static T[] ToBitArray<T>(this IEnumerable<byte> bytes, Endian endian = Endian.InheritFromHardware) => bytes.ToBitArray(endian).Cast<T>().ToArray();

        public static string ToBitArrayString(this int n, Endian endian = Endian.InheritFromHardware) => n.ToBitArray(endian).Cast<bool>().Select(bit => bit ? 1 : 0).JoinAsString();
        public static string ToBitArrayString(this long n, Endian endian = Endian.InheritFromHardware) => n.ToBitArray(endian).Cast<bool>().Select(bit => bit ? 1 : 0).JoinAsString();
        public static string ToBitArrayString(this byte b, Endian endian = Endian.InheritFromHardware) => b.ToBitArray(endian).Cast<bool>().Select(bit => bit ? 1 : 0).JoinAsString();
        public static string ToBitArrayString(this IEnumerable<byte> bytes, Endian endian = Endian.InheritFromHardware) => bytes.ToBitArray(endian).Cast<bool>().Select(bit => bit ? 1 : 0).JoinAsString();
        public static string ToBitArrayString(this IEnumerable<bool> bits) => bits.Select(bit => bit ? 1 : 0).JoinAsString();
        public static string ToBitArrayString(this BitArray ba) => ba.Cast<bool>().ToBitArrayString();

        public static byte[] ToByteArray(this BitArray ba, Endian endian = Endian.InheritFromHardware)
        {
            var byteArray = new byte[(ba.Length - 1) / 8 + 1];
            ba.CopyTo(byteArray, 0);
            return BitUtils.GetEndianIfInherited(endian) == Endian.LittleEndian ? byteArray : byteArray.Reverse().ToArray();
        }

        public static byte[] BitArrayToByteArray(this IEnumerable<bool> bits, Endian endian = Endian.InheritFromHardware) => bits.ToBitArray(endian).ToByteArray(endian);
        public static byte[] BitArrayToByteArray(this BitArray ba, Endian endian = Endian.InheritFromHardware) => ba.ToByteArray(endian);
        public static byte[] BitArrayToByteArray(this IEnumerable<byte> ba, Endian endian = Endian.InheritFromHardware) => new BitArray(ba.Select(bit => bit == 1).ToArray()).ToByteArray(endian);
        public static byte[] BitArrayToByteArray(this int[] ba, Endian endian = Endian.InheritFromHardware) => new BitArray(ba.Select(bit => bit == 1).ToArray()).ToByteArray(endian);
        public static byte[] BitArrayToByteArray(this string ba, Endian endian = Endian.InheritFromHardware) => new BitArray(ba.Select(bit => bit == 1).ToArray()).ToByteArray(endian);

        public static BitArray BitArrayStringToBitArray(this string s) => new (s.Select(c => c != '0').ToArray());

        public static int ToInt(this IEnumerable<bool> bits) => bits.BitArrayToByteArray().ToInt();

        public static bool[] ToVarInt(this int n, Endian endian, int varIntSizeBits = 5, int varIntSizeAdd = 0)
        {
            var ba = n.ToBitArray<bool>().EnforceLittleEndian(endian).ToArray();
            var varIntData = ba.SkipLastWhile(bit => !bit).ToArray();
            var varIntLength = (varIntData.Length - 1 + varIntSizeAdd).ToBitArray<bool>().Take(varIntSizeBits).ToArray(); // -1 by default, 32, max size of the integer should not exceed 5 bits so if we have 0 to use as well we can use numbers less one to represent size

            return varIntLength.Concat(varIntData).ToArray();
        }


        public static bool[] ToVarInt(this int n, int varIntSizeBits = 5, int varIntSizeAdd = 0)
        {
            return n.ToVarInt(Endian.InheritFromHardware, varIntSizeBits, varIntSizeAdd);
        }

        public static int GetFirstVarInt(this IEnumerable<byte> bytes, int startIndexOfVarInt = 0, int varIntSizeBits = 5, int varIntSizeAdd = 0)
        {
            var bits = bytes.ToBitArray<bool>();
            var varIntLengthAsInt = bits.Skip(startIndexOfVarInt).Take(varIntSizeBits).ToInt() + 1 - varIntSizeAdd;
            var varIntDataAsInt = bits.Skip(startIndexOfVarInt + varIntSizeBits).Take(varIntLengthAsInt).ToInt();
            return varIntDataAsInt;
        }

        public static int GetFirstVarIntLength(this IEnumerable<byte> bytes, int startIndexOfVarInt = 0, int varIntSizeBits = 5, int varIntSizeAdd = 0)
        {
            var bits = bytes.ToBitArray<bool>();
            return varIntSizeBits + bits.Skip(startIndexOfVarInt).Take(varIntSizeBits).ToInt() + 1 - varIntSizeAdd; // 5 bits to store size and the actual size parsed
        }
    }
}
