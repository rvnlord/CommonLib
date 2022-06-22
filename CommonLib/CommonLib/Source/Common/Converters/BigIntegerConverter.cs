using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using CommonLib.Source.Common.Extensions;
using Org.BouncyCastle.Math;

namespace CommonLib.Source.Common.Converters
{
    public static class BigIntegerConverter
    {
        public static BigInteger ToBigInt(this byte[] arr) => new BigInteger(arr);
        public static BigInteger ToBigIntU(this byte[] arr) => new BigInteger(1, arr);
        public static BigInteger ToBigInt(this IEnumerable<byte> en) => new BigInteger(en.ToArray());
        public static BigInteger ToBigIntU(this IEnumerable<byte> en) => new BigInteger(1, en.ToArray());

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Deliberate fall through to null on error")]
        public static BigInteger ToBigIntN(this object o)
        {
            if (o == null) return null;
            if (o is bool) return new BigInteger(Convert.ToInt32(o, CultureInfo.InvariantCulture).ToStringInvariant());
            try
            {
                var isNegative = false;
                var strBi = o.ToString();
                if (strBi.StartsWithInvariant("-"))
                {
                    strBi = strBi.Skip(1);
                    isNegative = true;
                }

                if (strBi.StartsWithInvariant("0x"))
                    strBi = strBi.Skip(2);
                var radix = 10;
                if (strBi.ContainsAny("ABCDEFabcdef".Select(c => c.ToStringInvariant()).ToArray()))
                    radix = 16;

                var bi = new BigInteger(strBi.ReplaceInvariant(".", ",").TakeUntil(","), radix);
                return isNegative ? bi.Negate() : bi;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static BigInteger ToBigInt(this object o)
        {
            var bigintN = o.ToBigIntN();
            if (bigintN != null) return bigintN;
            throw new ArgumentNullException(nameof(o));
        }
    }
}
