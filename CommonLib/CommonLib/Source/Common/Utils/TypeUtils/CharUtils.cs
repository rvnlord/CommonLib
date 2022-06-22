using System;
using System.Linq;

namespace CommonLib.Source.Common.Utils.TypeUtils
{
    public static class CharUtils
    {
        public static byte CharacterToByte(char character, int index, int shift = 0)
        {
            var value = (byte)character;
            switch (value)
            {
                case > 0x40 and < 0x47:
                case > 0x60 and < 0x67:
                {
                    if (0x40 != (0x40 & value))
                        return value;

                    if (0x20 == (0x20 & value))
                        value = (byte)((value + 0xA - 0x61) << shift);
                    else
                        value = (byte)((value + 0xA - 0x41) << shift);
                    break;
                }
                case > 0x29 and < 0x40:
                    value = (byte)((value - 0x30) << shift);
                    break;
                default:
                    throw new InvalidOperationException($"Character '{character}' at index '{index}' is not valid alphanumeric character.");
            }

            return value;
        }

    }
}
