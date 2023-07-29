using System;
using System.Linq;
using CommonLib.Source.Common.Utils;
using NBitcoin.DataEncoders;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace CommonLib.Source.Common.Converters
{
    public static class TendermintConverter
    {
        public static byte[] TendermintPrivateKeyToTendermintPublicKey(this byte[] privKey)
        {
            if (privKey.Length != 64)
                throw new FormatException("Tendermint private key should be 64 bytes long");
            
            AsymmetricKeyParameter privKeyparam = new Ed25519PrivateKeyParameters(privKey, 0);
            var pubKey = ((Ed25519PrivateKeyParameters)privKeyparam).GeneratePublicKey().GetEncoded();
            return pubKey;
        }

        public static byte[] TendermintPublicKeyToTendermintAddress(this byte[] pubKey)
        {
            if (pubKey.Length != 32)
                throw new FormatException("Tendermint public key should be 32 bytes long");
            
            return pubKey.Sha256().Take(20).ToArray();
        }
        
        public static byte[] TendermintPrivateKeyToTendermintAddress(this byte[] privKey) => privKey.TendermintPrivateKeyToTendermintPublicKey().TendermintPublicKeyToTendermintAddress();
    }
}
