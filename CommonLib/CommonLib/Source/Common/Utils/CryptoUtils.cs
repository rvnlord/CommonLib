using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Cryptography;
using CommonLib.Source.Common.Extensions.Collections;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Agreement.Kdf;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using EthECDSASignature = CommonLib.Source.Common.Cryptography.EthECDSASignature;

namespace CommonLib.Source.Common.Utils
{
    public static class CryptoUtils
    {
        public static string UniqueId()
        {
            return Guid.NewGuid().ToString();
        }

        public static AsymmetricCipherKeyPair GenerateECKeyPair()
        {
            var curve = ECNamedCurveTable.GetByName("secp256k1");
            var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
            var sr = new SecureRandom();
            var keyParams = new ECKeyGenerationParameters(domainParams, sr);
            var generator = new ECKeyPairGenerator("ECDSA");
            generator.Init(keyParams);
            return generator.GenerateKeyPair();
        }

        public static AsymmetricCipherKeyPair CreateECKeyPair(byte[] privKey)
        {
            var curve = ECNamedCurveTable.GetByName("secp256k1");
            var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
            var d = privKey.ToBigIntU();
            var ecPrivKey = new ECPrivateKeyParameters(d, domainParams);
            var q = domainParams.G.Multiply(d);
            var ecPubKey = new ECPublicKeyParameters(q, domainParams);
            return new AsymmetricCipherKeyPair(ecPubKey, ecPrivKey);
        }

        public static byte[] SignECDSA(this byte[] data, AsymmetricKeyParameter privKey)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            var signer = SignerUtilities.GetSigner("SHA256withECDSA");
            signer.Init(true, privKey);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.GenerateSignature();
        }

        public static bool VerifyECDSA(this byte[] data, byte[] signature, AsymmetricKeyParameter pubKey)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var ecdsaVerify = SignerUtilities.GetSigner("SHA256withECDSA");
            ecdsaVerify.Init(false, pubKey);
            ecdsaVerify.BlockUpdate(data, 0, data.Length);
            return ecdsaVerify.VerifySignature(signature);
        }

        public static EthECDSASignature SignEthECDSA(AsymmetricKeyParameter privKey, byte[] data)
        {
            var digest = new Sha256Digest();
            var signer = new ECDsaSigner(new HMacDsaKCalculator(digest));
            signer.Init(true, privKey);
            return new EthECDSASignature(signer.GenerateSignature(data)).CalculateV(privKey.ECPrivateKeyToECPublicKey(), data);
        }

        public static bool VerifyEthECDSA(AsymmetricKeyParameter pubKey, EthECDSASignature signature, byte[] data)
        {
            if (signature == null)
                throw new ArgumentNullException(nameof(signature));

            var signer = new ECDsaSigner();
            signer.Init(false, pubKey);
            return signer.VerifySignature(data, signature.R.ToBigIntU(), signature.S.ToBigIntU());
        }

        public static byte[] Ripemd160(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var digest = new RipeMD160Digest();
            var output = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(data, 0, data.Length);
            digest.DoFinal(output, 0);
            return output;
        }

        public static byte[] SignHMACSha512(byte[] key, byte[] data)
        {
            var nmacsha512Creator = new HMACSHA512(key);
            var hmacsha512 = nmacsha512Creator.ComputeHash(data);
            nmacsha512Creator.Dispose();
            return hmacsha512;
        }

        public static byte[] SignHMACSha384(byte[] key, byte[] data)
        {
            var nmacsha384Creator = new HMACSHA384(key);
            var hmacsha384 = nmacsha384Creator.ComputeHash(data);
            nmacsha384Creator.Dispose();
            return hmacsha384;
        }

        public static byte[] SignHMACSha256(byte[] key, byte[] data)
        {
            var nmacsha256Creator = new HMACSHA256(key);
            var hmacsha256 = nmacsha256Creator.ComputeHash(data);
            nmacsha256Creator.Dispose();
            return hmacsha256;
        }

        public static byte[] Sha256(byte[] value)
        {
            var sha256Creator = SHA256.Create();
            var sha256 = sha256Creator.ComputeHash(value);
            sha256Creator.Dispose();
            return sha256;
        }

        public static string Sha3(this string value) => Sha3(value.UTF8ToByteArray()).ToHexString();

        public static byte[] Sha3(this byte[] value)
        {

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var digest = new KeccakDigest(256);
            var output = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(value, 0, value.Length);
            digest.DoFinal(output, 0);
            return output;
        }

        public static byte[] Sha3(this List<byte> value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return Sha3(value.ToArray());
        }

        public static byte[] GenerateCamelliaKey()
        {
            var random = new SecureRandom();
            var genParam = new KeyGenerationParameters(random, 256);
            var kGen = GeneratorUtilities.GetKeyGenerator("CAMELLIA");
            kGen.Init(genParam);
            return kGen.GenerateKey(); ;
        }

        public static byte[] EncryptCamellia(this byte[] data, byte[] key)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            var cipher = CipherUtilities.GetCipher("CAMELLIA");
            cipher.Init(true, ParameterUtilities.CreateKeyParameter("CAMELLIA", key));
            return cipher.DoFinal(data);
        }

        public static byte[] DecryptCamellia(this byte[] data, byte[] key)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            var cipher = CipherUtilities.GetCipher("CAMELLIA");
            cipher.Init(false, ParameterUtilities.CreateKeyParameter("CAMELLIA", key));
            return cipher.DoFinal(data);
        }

        private static byte[] GetECCSharedSecret(this AsymmetricCipherKeyPair ecKeyPair)
        {
            var agreement = new ECDHCBasicAgreement();
            agreement.Init(ecKeyPair.Private);
            return agreement.CalculateAgreement(ecKeyPair.Public).ToByteArray();
        }

        //private static byte[] GetECCSharedSecretForEncryption(this AsymmetricCipherKeyPair ecKeyPair) => GetECCSharedSecret(Operation.Encryption, ecKeyPair);
        //private static byte[] GetECCSharedSecretForDecryption(this AsymmetricCipherKeyPair ecKeyPair) => GetECCSharedSecret(Operation.Decryption, ecKeyPair);

        private static byte[] GetECCKeyFromSharedSecret(this byte[] sharedSecret)
        {
            var egH = new ECDHKekGenerator(DigestUtilities.GetDigest("SHA256"));
            egH.Init(new DHKdfParameters(NistObjectIdentifiers.Aes, sharedSecret.Length, sharedSecret));
            var symmetricKey = new byte[DigestUtilities.GetDigest("SHA256").GetDigestSize()];
            egH.GenerateBytes(symmetricKey, 0, symmetricKey.Length);
            return symmetricKey;
        }
        
        private static byte[] EncryptDecryptECCWithKeyFromSharedSecret(Operation operation, byte[] keyFromSharedSecret, byte[] data)
        {
            //var keyParam = ParameterUtilities.CreateKeyParameter("DES", keyFromSharedSecret);
            //var cipher = CipherUtilities.GetCipher("AES/CFB/ISO7816_4PADDING");
            var keyParam = new KeyParameter(keyFromSharedSecret, 0, keyFromSharedSecret.Length);
            var cipher = new PaddedBufferedBlockCipher(new CfbBlockCipher(new KuznyechikEngine(), KuznyechikEngine.BlockSize), new ISO7816d4Padding());
            cipher.Init(operation == Operation.Encryption, keyParam);
            return cipher.DoFinal(data);
        }

        private static byte[] EncryptECCWithKeyFromSharedSecret(byte[] keyFromSharedSecret, byte[] plainData) => EncryptDecryptECCWithKeyFromSharedSecret(Operation.Encryption, keyFromSharedSecret, plainData);
        private static byte[] DecryptECCWithKeyFromSharedSecret(byte[] keyFromSharedSecret, byte[] cipheredData) => EncryptDecryptECCWithKeyFromSharedSecret(Operation.Decryption, keyFromSharedSecret, cipheredData);
        
        public static byte[] EncryptECC(this byte[] plainData, byte[] senderPrivateKey, byte[] receiverPublicKey)
        {
            var privKey = senderPrivateKey.ECPrivateKeyByteArrayToECPrivateKey();
            var pubKey = receiverPublicKey.ECPublicKeyByteArrayToECPublicKey();
            var keyPair = new AsymmetricCipherKeyPair(pubKey, privKey);
            var encryptionKey = keyPair.GetECCSharedSecret().GetECCKeyFromSharedSecret();
            return EncryptECCWithKeyFromSharedSecret(encryptionKey, plainData);
        }

        public static byte[] DecryptECC(this byte[] cipheredData, byte[] receiverPrivateKey, byte[] senderPublicKey)
        {
            var privKey = receiverPrivateKey.ECPrivateKeyByteArrayToECPrivateKey();
            var pubKey = senderPublicKey.ECPublicKeyByteArrayToECPublicKey();
            var keyPair = new AsymmetricCipherKeyPair(pubKey, privKey);
            var decryptionKey = keyPair.GetECCSharedSecret().GetECCKeyFromSharedSecret();
            return DecryptECCWithKeyFromSharedSecret(decryptionKey, cipheredData);
        }
        
        //SImplify ECC so priv key is sender ec priv + receiver ec pub and pub key is receiver ec priv + sender ec pub
        public static PrivateKeyPair GenerateECCKeyPair()
        {
            var senderKeyPair = GenerateECKeyPair();
            var receiverKeyPair = GenerateECKeyPair();
            var eccPerson1PrivKey = senderKeyPair.Private.ToECPrivateKeyByteArray().Concat(receiverKeyPair.Public.ToECPublicKeyByteArray());
            var eccPerson2PrivKey = receiverKeyPair.Private.ToECPrivateKeyByteArray().Concat(senderKeyPair.Public.ToECPublicKeyByteArray());
            return new PrivateKeyPair(eccPerson1PrivKey, eccPerson2PrivKey);
        }

        public static byte[] EncryptECC(this byte[] plainData, byte[] privateKey)
        {
            var senderPrivateKey = privateKey.Take(32).ToArray();
            var receiverPublicKey = privateKey.Skip(32).ToArray();
            return plainData.EncryptECC(senderPrivateKey, receiverPublicKey);
        }

        public static byte[] DecryptECC(this byte[] cipheredData, byte[] publicKey)
        {
            var receiverPrivateKey = publicKey.Take(32).ToArray();
            var senderPublicKey = publicKey.Skip(32).ToArray();
            return cipheredData.DecryptECC(receiverPrivateKey, senderPublicKey);
        }
    }

    public enum Operation
    {
        Encryption,
        Decryption
    }

    public struct PrivateKeyPair
    {
        public byte[] Person1Private { get; set; }
        public byte[] Person2Private { get; set; }

        public PrivateKeyPair(byte[] person1Private, byte[] person2Private)
        {
            Person1Private = person1Private;
            Person2Private = person2Private;
        }
    }
}
