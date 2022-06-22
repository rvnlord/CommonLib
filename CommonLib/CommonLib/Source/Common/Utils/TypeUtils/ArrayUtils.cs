using System.Linq;

namespace CommonLib.Source.Common.Utils.TypeUtils
{
    public static class ArrayUtils
    {
        public static T[] ConcatMany<T>(params T[][] arrays) => arrays.SelectMany(x => x).ToArray();
    }
}
