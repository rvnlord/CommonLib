using System.Linq;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Utils
{
    public static class BitcoinUtils
    {
        public static bool IsCorrectBitcoinAddress(this string address) => address.IsValidAsCompressed(25);
        public static bool IsCorrectBitcoinCompressedPrivateKey(this string privKey) => privKey.IsValidAsCompressed(38);

        private static bool IsValidAsCompressed(this string str, int length)
        {
            if (str.IsNullOrEmpty() || !str.IsBase58())
                return false;

            var arr = str.Base58ToByteArray();
            if (arr.Length != length)
                return false;

            var beforeCheckSum = arr.SkipLast(4).ToArray();
            var expectedCheckSum = str.Base58ToByteArray().TakeLast(4).ToArray();
            var calculatedCheckSUm = CryptoUtils.Sha256(CryptoUtils.Sha256(beforeCheckSum)).Take(4).ToArray();
            return expectedCheckSum.SequenceEqual(calculatedCheckSUm);
        }
    }
}
