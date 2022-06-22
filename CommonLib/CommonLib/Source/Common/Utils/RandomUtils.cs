﻿using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions;
using static CommonLib.Source.LibConfig;

namespace CommonLib.Source.Common.Utils
{
    public static class RandomUtils
    {
        public static int RandomIntBetween(int min, int max)
        {
            return _r.Next(min, max + 1);
        }

        public static decimal RandomDecimalBetween(decimal min, decimal max, int decimals = 8)
        {
            return (_r.NextDouble() * (max.ToDouble() - min.ToDouble()) + min.ToDouble()).ToDecimal().Round(decimals);
        }
    }
}
