using System;
using CommonLib.Source.Common.Converters;

namespace CommonLib.Source.Common.Utils
{
    public static class BitUtils
    {
        public static int MaxNumberStoredForBits(int bits)
            //=> (int)(Math.Pow(2, bits) - 1);
            => (1 << bits) - 1;

        public static Endian GetEndianIfInherited(Endian endian)
        {
            if (endian == Endian.InheritFromHardware)
                endian = GetSystemEndian();
            return endian;
        }

        public static Endian GetSystemEndian() => BitConverter.IsLittleEndian ? Endian.LittleEndian : Endian.BigEndian;
    }
}
