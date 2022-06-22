using System;
using CommonLib.Source.Common.Converters;

namespace CommonLib.Source.Common.Extensions
{
    public static class IntExtensions
    {
        public static int Abs(this int n) => Math.Abs(n);
        public static bool Between(this int n, int lower, int upper) => n.ToDouble().Between(lower, upper);
    }
}
