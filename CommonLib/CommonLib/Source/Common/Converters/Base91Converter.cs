using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib.Source.Common.Utils.TypeUtils;

namespace CommonLib.Source.Common.Converters
{
    public static class Base91Converter
    {
        private static readonly string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#$%&()*+,./:;<=>?@[]^_`{|}~\"";
        private static readonly int[] _invAlphabet = StringUtils.ReverseLookupAlphabet(_alphabet);
        
        public static string ToBase91String(this byte[] data)
        {
            var result = new StringBuilder(data.Length);
            int ebq = 0, en = 0;
            foreach (var b in data)
            {
                ebq |= (b & 255) << en;
                en += 8;
                if (en <= 13) 
                    continue;

                var ev = ebq & 8191;

                if (ev > 88)
                {
                    ebq >>= 13;
                    en -= 13;
                }
                else
                {
                    ev = ebq & 16383;
                    ebq >>= 14;
                    en -= 14;
                }

                var quotient = Math.DivRem(ev, 91, out var remainder);
                result.Append(_alphabet[remainder]);
                result.Append(_alphabet[quotient]);
            }

            if (en > 0)
            {
                var quotient = Math.DivRem(ebq, 91, out var remainder);
                result.Append(_alphabet[remainder]);
                if (en > 7 || ebq > 90)
                    result.Append(_alphabet[quotient]);
            }

            return result.ToString();
        }

        public static byte[] Base91ToByteArray(this string data)
        {
            unchecked
            {
                int dbq = 0, dn = 0, dv = -1;
                var result = new List<byte>(data.Length);
                foreach (var b in data.Where(b => _invAlphabet[b] != -1))
                {
                    if (dv == -1)
                        dv = _invAlphabet[b];
                    else
                    {
                        dv += _invAlphabet[b] * 91;
                        dbq |= dv << dn;
                        dn += (dv & 8191) > 88 ? 13 : 14;
                        do
                        {
                            result.Add((byte)dbq);
                            dbq >>= 8;
                            dn -= 8;
                        }
                        while (dn > 7);
                        dv = -1;
                    }
                }

                if (dv != -1)
                    result.Add((byte)(dbq | dv << dn));

                return result.ToArray();
            }
        }
    }
}
