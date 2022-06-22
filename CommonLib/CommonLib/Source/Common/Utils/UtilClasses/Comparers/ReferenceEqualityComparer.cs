using System;
using System.Collections.Generic;

namespace CommonLib.Source.Common.Utils.UtilClasses.Comparers
{
    public class CustomReferenceEqualityComparer : EqualityComparer<object>
    {
        public override bool Equals(object x, object y) => ReferenceEquals(x, y);
        public override int GetHashCode(object obj)
        {
            if (obj == null) 
                throw new ArgumentNullException(nameof(obj));
            return obj.GetHashCode();
        }
    }
}
