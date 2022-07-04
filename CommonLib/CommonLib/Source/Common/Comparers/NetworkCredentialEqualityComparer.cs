using System;
using System.Collections.Generic;
using System.Net;

namespace CommonLib.Source.Common.Comparers
{
    public class NetworkCredentialEqualityComparer : IEqualityComparer<NetworkCredential>
    {
        public bool Equals(NetworkCredential x, NetworkCredential y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Domain == y.Domain && x.Password == y.Password && x.UserName == y.UserName;
        }

        public int GetHashCode(NetworkCredential obj) => HashCode.Combine(obj.Domain, obj.Password, obj.UserName);
    }
}
