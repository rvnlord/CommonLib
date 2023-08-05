using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Source.Common.Cryptography;
using CommonLib.Source.Common.Extensions;
using CommonLib.Source.Common.Utils;
using Nethereum.Signer.Crypto;
using Nethereum.Util;

namespace CommonLib.Source.Common.Converters
{
    public static class EthereumConverter
    {
        // byte[] (ECDSA Signature) --> ECDSASignature (ECDSA Signature)
        public static EthECDSASignature ECDSASignatureByteArrayToECDSASignature(this byte[] sig)
        {
            return new EthECDSASignature(sig);
        }

        public static byte[] ECPublicKeyByteArrayToEthereumAddressByteArray(this byte[] pubKey)
        {
            var initaddr = pubKey.Keccak256();
            var addr = new byte[initaddr.Length - 12];
            Array.Copy(initaddr, 12, addr, 0, initaddr.Length - 12);
            return addr;
        }
        
        public static string EthereumAddressByteArrayToEthereumAddressChecksumString(this byte[] bytesAddress)
        {
            var address = bytesAddress.ToHexString().ToLower().RemoveHexPrefix();
            var addressHash = address.Keccak256();
            var checksumAddress = new StringBuilder("0x");

            for (var i = 0; i < address.Length; i++)
                checksumAddress.Append(int.Parse(addressHash[i].ToString(), NumberStyles.HexNumber) > 7 ? address[i].ToString().ToUpper() : address[i].ToString().ToLower());
            return checksumAddress.ToString();
        }
    }
}
