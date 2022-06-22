using System;
using System.Linq;
using CommonLib.Source.Common.Cryptography;
using CommonLib.Source.Common.Extensions;
using CommonLib.Source.Common.Utils;
using MoreLinq;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace CommonLib.Source.Common.Converters
{
    public static class CryptographyConverter
    {
        // AsymmetricKeyParameter (EC Private key) --> byte[] (EC Private key)
        public static byte[] ToECPrivateKeyByteArray(this AsymmetricKeyParameter ecPrivateKey)
        {
            if (ecPrivateKey == null)
                throw new ArgumentNullException(nameof(ecPrivateKey));

            return ((ECPrivateKeyParameters)ecPrivateKey).D.ToString(16).HexToByteArray().PadStart<byte>(32, 0x00).ToArray();
        }

        // byte[] (EC Private key) --> AsymmetricKeyParameter (EC Private key)
        public static AsymmetricKeyParameter ECPrivateKeyByteArrayToECPrivateKey(this byte[] ecPrivateKey)
        {
            return CryptoUtils.CreateECKeyPair(ecPrivateKey).Private;
        }

        // AsymmetricKeyParameter (EC Public key) --> byte[] (EC Public key)
        public static byte[] ToECPublicKeyByteArray(this AsymmetricKeyParameter ecPublicKey)
        {
            if (ecPublicKey == null)
                throw new ArgumentNullException(nameof(ecPublicKey));

            var publicKey = ((ECPublicKeyParameters)ecPublicKey).Q;
            var xs = publicKey.AffineXCoord.ToBigInteger().ToByteArrayUnsigned().PadStart(32);
            var ys = publicKey.AffineYCoord.ToBigInteger().ToByteArrayUnsigned().PadStart(32);
            return xs.Concat(ys).ToArray();
        }

        // byte[] (EC Public key) --> AsymmetricKeyParameter (EC Public key)
        public static AsymmetricKeyParameter ECPublicKeyByteArrayToECPublicKey(this byte[] ecPublicKey)
        {
            var curve = ECNamedCurveTable.GetByName("secp256k1");
            var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
            var x = ecPublicKey.Take(32).ToBigIntU();
            var y = ecPublicKey.Skip(32).ToBigIntU();
            var q = curve.Curve.CreatePoint(x, y);
            return new ECPublicKeyParameters(q, domainParams);
        }

        // byte[] (EC Private key) --> byte[] (EC Public key)
        public static byte[] ECPrivateKeyByteArrayToECPublicKeyByteArray(this byte[] ecPrivateKey)
        {
            return CryptoUtils.CreateECKeyPair(ecPrivateKey).Public.ToECPublicKeyByteArray();
        }

        // AsymmetricKeyParameter (EC Private key) --> AsymmetricKeyParameter (EC Public key)
        public static AsymmetricKeyParameter ECPrivateKeyToECPublicKey(this AsymmetricKeyParameter ecPrivateKey)
        {
            return CryptoUtils.CreateECKeyPair(ecPrivateKey.ToECPrivateKeyByteArray()).Public;
        }

        // byte[] (ECDSA Signature) --> ECDSASignature (ECDSA Signature)
        public static EthECDSASignature ECDSASignatureByteArrayToECDSASignature(this byte[] sig)
        {
            return new EthECDSASignature(sig);
        }

        // string (EC Private Key) --> AsymmetricKeyParameter (EC Private Key)
        public static AsymmetricKeyParameter ECPrivateKeyStringToECPrivateKey(this string ecPrivateKey)
        {
            return ecPrivateKey.RemoveHexPrefix().HexToByteArray().ECPrivateKeyByteArrayToECPrivateKey();
        }
    }
}
