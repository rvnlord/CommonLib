using System;
using CommonLib.Source.Common.Converters;
using Org.BouncyCastle.Math;

namespace CommonLib.Source.Common.Extensions
{
    public static class BigIntegerExtensions
    {
        public static bool IsEven(this BigInteger number)
        {
            if (number == null)
                throw new ArgumentNullException(nameof(number));

            return number.GetLowestSetBit() != 0;
        }

        public static BigInteger FloorDivide(this BigInteger a, BigInteger b)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            if (a.CompareTo(0.ToBigInt()) > 0 ^ b.CompareTo(0.ToBigInt()) < 0 && !a.Mod(b).Equals(0.ToBigInt()))
                return a.Divide(b).Subtract(1.ToBigIntN());

            return a.Divide(b);
        }
    }
}
