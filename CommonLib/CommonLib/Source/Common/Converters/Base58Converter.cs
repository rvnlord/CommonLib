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
        private const string _alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static string HexToBase58(this string str) => str.HexToByteArray().ToBase58StringLegacy();
        public static string UTF8ToBase58(this string utf8str, bool throwIfInputIsAlreadyBase58 = false) => throwIfInputIsAlreadyBase58 && utf8str.IsBase58() ? throw new FormatException("Input is already a base58 string") : utf8str.UTF8ToByteArray().ToBase58String();
        public static string Base58ToUTF8(this string base58) => base58.Base58ToByteArray().ToUTF8String();

        public static byte[] Base58ToByteArrayLegacy(this string encoded)
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

        public static string ToBase58StringLegacy(this byte[] data, int offset, int count)
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

        public static string ToBase58String(this IEnumerable<byte> bytes)
        {
            var arrBytes = bytes.ToArray();
            var d = new List<byte>();
            var s = new StringBuilder();
            var j = 0;
            for (var i = 0; i < arrBytes.Length; i++)
            {
                j = 0;
                int c = arrBytes[i];
                if (c == 0 && (s.Length ^ i) == 0) 
                    s.Append('1');

                while (j < d.Count || c != 0)
                {
                    var n = j >= d.Count ? -1 : d[j];                  
                    n = n > 0 ? n * 256 + c : c;     
                    c = n / 58;             
                    d.InsertOrUpdate(j, (byte)(n % 58));
                    j++;
                }
            }

            while (j-- > 0)
                s.Append(_alphabet[d[j]]);

            return s.ToString();
        }

        public static byte[] Base58ToByteArray(this string base58Str)
        {
            var d = new List<byte>();  
            var b = new List<byte>();
            var j = 0;
            for (var i = 0; i < base58Str.Length; i++) 
            {
                j = 0;
                var c = _alphabet.IndexOf(base58Str[i]);
                if (c < 0)
                    throw new ArgumentOutOfRangeException(nameof(c));
                if (c == 0 && (b.Count ^ i) == 0) 
                    b.Add(0);

                while (j < d.Count || c != 0) 
                {
                    var n = j >= d.Count ? -1 : d[j];    
                    n = n > 0 ? n * 58 + c : c;
                    c = n >> 8;
                    d.InsertOrUpdate(j, (byte)(n % 256));
                    j++;
                }
            }

            while (j-- > 0)
                b.Add(d[j]);

            return b.ToArray();
        }
        
        public static string ToBase58StringLegacy(this IEnumerable<byte> data)
        {
            var arrData = data.ToArray();
            return arrData.ToBase58StringLegacy(0, arrData.Length);
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

        public static bool IsBase58(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            return str.All(c => PszBase58.ContainsInvariant(c));
        }
    }
}
