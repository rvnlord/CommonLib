using System;

namespace CommonLib.Source.Common.Converters
{
    public static class ArrayConverter
    {
        public static sbyte[] ToSigned(this byte[] arr) => (sbyte[])(Array)arr;

    }
}
