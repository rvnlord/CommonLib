using System;
using System.Globalization;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Converters
{
    public static class UIntConverter
    {
        public static uint? ToUIntN(this object o)
        {
            if (o == null) return null;
            if (o is bool) return Convert.ToUInt32(o, CultureInfo.InvariantCulture);
            if (o.GetType().IsEnum) return (uint)o;
            return uint.TryParse(o.ToDoubleN()?.Round().ToStringInvariant().BeforeFirstOrWhole("."), NumberStyles.Any, CultureInfo.InvariantCulture, out var val) ? val : (uint?)null;
        }

        public static uint ToUInt(this object o)
        {
            var uintN = o.ToUIntN();
            if (uintN != null) return (uint)uintN;
            throw new ArgumentNullException(nameof(o));
        }
    }
}
