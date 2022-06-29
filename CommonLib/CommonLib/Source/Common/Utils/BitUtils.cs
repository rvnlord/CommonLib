using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonLib.Source.Common.Converters;

namespace CommonLib.Source.Common.Utils
{
    public static class BitUtils
    {
        public static int MaxSizeStoredForBits(int bits) => 1 << bits;

        public static int MaxNumberStoredForBits(int bits) 
            //=> (int)(Math.Pow(2, bits) - 1);
            => MaxSizeStoredForBits(bits) - 1;

        public static Endian GetEndianIfInherited(Endian endian)
        {
            if (endian == Endian.InheritFromHardware)
                endian = GetSystemEndian();
            return endian;
        }
        
        public static BitArray EnforceLittleEndian(this BitArray ba, Endian endian = Endian.InheritFromHardware)
        {
            endian = GetEndianIfInherited(endian);
            return endian == Endian.LittleEndian ? ba : new BitArray(ba.Cast<bool>().Reverse().ToArray());
        }

        public static IEnumerable<bool> EnforceLittleEndian(this IEnumerable<bool> ba, Endian endian = Endian.InheritFromHardware)
        {
            endian = GetEndianIfInherited(endian);
            return endian == Endian.LittleEndian ? ba : ba.Reverse().ToArray();
        }

        public static Endian GetSystemEndian() => BitConverter.IsLittleEndian ? Endian.LittleEndian : Endian.BigEndian;
    }
}
