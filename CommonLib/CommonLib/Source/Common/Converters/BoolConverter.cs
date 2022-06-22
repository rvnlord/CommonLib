using System;

namespace CommonLib.Source.Common.Converters
{
    public static class BoolConverter
    {
        public static bool? ToBoolN(this object o)
        {
            if (o == null) return null;
            if (o is bool) return (bool)o;
            if (o.ToIntN() != null) return Convert.ToBoolean(o.ToInt());
            return bool.TryParse(o.ToString(), out var tmpvalue) ? tmpvalue : (bool?)null;
        }

        public static bool ToBool(this object o)
        {
            var boolN = o.ToBoolN();
            if (boolN != null) return (bool)boolN;
            throw new ArgumentNullException(nameof(o));
        }
    }
}
