using System.Collections.Generic;
using System.Linq;
using CommonLib.Source.Common.Converters;

namespace CommonLib.Source.Common.Utils.TypeUtils
{
    public static class ByteUtils
    {
        public static int MaxSizeStoredForBytes(int bytes) => BitUtils.MaxSizeStoredForBits(bytes * 8);
        public static int MaxNumberStoredForBytes(int bytes) => MaxSizeStoredForBytes(bytes) - 1;

        public static IEnumerable<byte> EnforceLittleEndian(this IEnumerable<byte> ba, Endian endian = Endian.InheritFromHardware)
        {
            endian = BitUtils.GetEndianIfInherited(endian);
            return endian == Endian.LittleEndian ? ba : ba.Reverse().ToArray();
        }
    }
}
