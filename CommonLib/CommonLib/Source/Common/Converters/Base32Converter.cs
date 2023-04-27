using System;
using System.Collections.Generic;
using System.Linq;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Converters
{
    public static class Base32Converter
    {
        private const string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        public static string ToBase32String(this IEnumerable<byte> bytes)
        {
            var arrBytes = bytes?.ToArray();
            if (arrBytes is null || !arrBytes.Any())
                return "";
            
            var outputChars = new char[(arrBytes.Length * 8 + 4) / 5];
            var inputBytes = new byte[5];
            for (int i = 0, outputIndex = 0; i < arrBytes.Length; i += 5, outputIndex += 8)
            {
                var count = Math.Min(5, arrBytes.Length - i);
                Array.Copy(arrBytes, i, inputBytes, 0, count);
                
                outputChars[outputIndex] = _alphabet[inputBytes[0] >> 3];
                outputChars[outputIndex + 1] = _alphabet[(inputBytes[0] << 2 | inputBytes[1] >> 6) & 0x1F];
                outputChars[outputIndex + 2] = _alphabet[(inputBytes[1] >> 1) & 0x1F];
                outputChars[outputIndex + 3] = _alphabet[(inputBytes[1] << 4 | inputBytes[2] >> 4) & 0x1F];
                outputChars[outputIndex + 4] = _alphabet[(inputBytes[2] << 1 | inputBytes[3] >> 7) & 0x1F];
                outputChars[outputIndex + 5] = _alphabet[(inputBytes[3] >> 2) & 0x1F];
                outputChars[outputIndex + 6] = _alphabet[(inputBytes[3] << 3 | inputBytes[4] >> 5) & 0x1F];
                outputChars[outputIndex + 7] = _alphabet[inputBytes[4] & 0x1F];

                if (count >= 5) 
                    continue;

                var paddingCount = 5 - count;
                for (var j = 0; j < paddingCount; j++)
                    outputChars[outputIndex + 8 - paddingCount + j] = '=';
            }

            return new string(outputChars);
        }

        public static byte[] Base32ToByteArray(this string base32Str)
        {
            if (base32Str.IsNullOrWhiteSpace())
                return Array.Empty<byte>();
            
            base32Str = base32Str.TrimEnd('=');
            var outputBytes = new byte[base32Str.Length * 5 / 8];
            var inputBytes = new byte[8];
            for (int i = 0, outputIndex = 0; i < base32Str.Length; i += 8, outputIndex += 5)
            {
                for (var j = 0; j < 8; j++)
                {
                    var charIndex = i + j;
                    byte charValue = 0; 
                    if (charIndex < base32Str.Length)
                    {
                        int c = base32Str[charIndex];
                        charValue = c switch
                        {
                            >= 'A' and <= 'Z' => (byte)(c - 'A'),
                            >= '2' and <= '7' => (byte)(c - '2' + 26),
                            _ => throw new ArgumentException($"Invalid base32 character '{c}'.")
                        };
                    }
                    
                    inputBytes[j] = charValue;
                }
                
                outputBytes[outputIndex] = (byte)((inputBytes[0] << 3) | (inputBytes[1] >> 2)); // Convert 8 bytes to 5 bytes
                outputBytes[outputIndex + 1] = (byte)((inputBytes[1] << 6) | (inputBytes[2] << 1) | (inputBytes[3] >> 4));
                outputBytes[outputIndex + 2] = (byte)((inputBytes[3] << 4) | (inputBytes[4] >> 1));
                outputBytes[outputIndex + 3] = (byte)((inputBytes[4] << 7) | (inputBytes[5] << 2) | (inputBytes[6] >> 3));
                outputBytes[outputIndex + 4] = (byte)((inputBytes[6] << 5) | inputBytes[7]);
            }

            return outputBytes;
        }
    }
}
