using System.Net;
using CommonLib.Source.Common.Comparers;

namespace CommonLib.Source.Common.Extensions
{
    public static class NetworkCredentialExtensions
    {
        public static bool Equals_(this NetworkCredential x, NetworkCredential y) => new NetworkCredentialEqualityComparer().Equals(x, y);
    }
}
