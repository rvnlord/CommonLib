using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib.Source.Common.Extensions;
using CommonLib.Source.Common.Extensions.Collections;
using Org.BouncyCastle.Math;
using static CommonLib.Source.LibConfig;

namespace CommonLib.Source.Common.Converters
{
    public static class Base58Converter
    {
        public static string HexToBase58(this string str) => str.HexToByteArray().ToBase58String();
        public static string UTF8ToBase58(this string utf8str, bool throwIfInputIsAlreadyBase58 = true) => throwIfInputIsAlreadyBase58 && utf8str.IsBase58() ? throw new FormatException("Input is already a base58 string") : utf8str.UTF8ToByteArray().ToBase58String();
        public static string Base58ToUTF8(this string base58) => base58.Base58ToByteArray().ToUTF8String();
        
        public static byte[] Base58ToByteArray(this string encoded)
        {
            if (encoded == null)
                throw new ArgumentNullException(nameof(encoded));

            var result = Array.Empty<byte>();
            if (encoded.Length == 0)
                return result;
            var bn = BigInteger.Zero;
            var i = 0;
            while (encoded[i].IsSpace())
            {
                i++;
                if (i >= encoded.Length)
                    return result;
            }

            for (var y = i; y < encoded.Length; y++)
            {
                var p1 = PszBase58.IndexOf_(encoded[y]);
                if (p1 == -1)
                {
                    while (encoded[y].IsSpace())
                    {
                        y++;
                        if (y >= encoded.Length)
                            break;
                    }
                    if (y != encoded.Length)
                        throw new FormatException("Invalid base 58 string");
                    break;
                }
                var bnChar = BigInteger.ValueOf(p1);
                bn = bn.Multiply(Bn58);
                bn = bn.Add(bnChar);
            }

            var vchTmp = bn.ToByteArrayUnsigned();
            Array.Reverse(vchTmp);
            if (vchTmp.All(b => b == 0))
                vchTmp = Array.Empty<byte>();

            if (vchTmp.Length >= 2 && vchTmp[^1] == 0 && vchTmp[^2] >= 0x80)
                vchTmp = vchTmp.SafeSubarray(0, vchTmp.Length - 1);

            var nLeadingZeros = 0;
            for (var y = i; y < encoded.Length && encoded[y] == PszBase58[0]; y++)
                nLeadingZeros++;

            result = new byte[nLeadingZeros + vchTmp.Length];
            Array.Copy(vchTmp.Reverse().ToArray(), 0, result, nLeadingZeros, vchTmp.Length);
            return result;
        }

        public static string ToBase58String(this byte[] data, int offset, int count)
        {
            var bn0 = BigInteger.Zero;
            var vchTmp = data.SafeSubarray(offset, count);
            var bn = new BigInteger(1, vchTmp);
            var builder = new StringBuilder();

            while (bn.CompareTo(bn0) > 0)
            {
                var r = bn.DivideAndRemainder(Bn58);
                var dv = r[0];
                var rem = r[1];
                bn = dv;
                var c = rem.IntValue;
                builder.Append(PszBase58[c]);
            }

            for (var i = offset; i < offset + count && data[i] == 0; i++)
                builder.Append(PszBase58[0]);

            var chars = builder.ToString().ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        public static string ToBase58String(this IEnumerable<byte> data)
        {
            var arrData = data.ToArray();
            return arrData.ToBase58String(0, arrData.Length);
        }

        public static string Base58ToUTF8OrNull(this string base58str)
        {
            try
            {
                return base58str.Base58ToUTF8();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
