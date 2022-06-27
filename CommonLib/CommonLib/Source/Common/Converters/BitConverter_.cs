using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonLib.Source.Common.Extensions.Collections;
using CommonLib.Source.Common.Utils;

namespace CommonLib.Source.Common.Converters
{
    public static class BitConverter_
    {
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

        public static byte[] BitArrayToByteArray(BitArray ba, Endian endian = Endian.InheritFromHardware) => ba.ToByteArray(endian);
        public static byte[] BitArrayToByteArray(IEnumerable<byte> ba, Endian endian = Endian.InheritFromHardware) => new BitArray(ba.Select(bit => bit == 1).ToArray()).ToByteArray(endian);
        public static byte[] BitArrayToByteArray(int[] ba, Endian endian = Endian.InheritFromHardware) => new BitArray(ba.Select(bit => bit == 1).ToArray()).ToByteArray(endian);
        public static byte[] BitArrayToByteArray(string ba, Endian endian = Endian.InheritFromHardware) => new BitArray(ba.Select(bit => bit == 1).ToArray()).ToByteArray(endian);

        public static BitArray BitArrayStringToBitArray(this string s) => new (s.Select(c => c != '0').ToArray());
    }
}
