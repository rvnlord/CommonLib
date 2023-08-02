using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions;
using NBitcoin.Secp256k1;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities.Encoders;

namespace CommonLib.Source.Common.Cryptography
{
    public class EthECDSASignature
    {
        private const string InvalidDERSignature = "Invalid DER signature";

        public EthECDSASignature(BigInteger r, BigInteger s)
        {
            R = r;
            S = s;
        }

        public EthECDSASignature(BigInteger[] rs)
        {
            R = rs[0];
            S = rs[1];
        }

        public EthECDSASignature(BigInteger r, BigInteger s, byte[] v)
        {
            R = r;
            S = s;
            V = v;
        }

        public EthECDSASignature(byte[] derSig)
        {
            try
            {
                var decoder = new Asn1InputStream(derSig);
                if (decoder.ReadObject() is not DerSequence { Count: 2 } seq)
                    throw new FormatException(InvalidDERSignature);
                R = ((DerInteger)seq[0]).Value;
                S = ((DerInteger)seq[1]).Value;
            }
            catch (Exception ex)
            {
                throw new FormatException(InvalidDERSignature, ex);
            }
        }


        public static EthECDSASignature ExtractECDSASignature(byte[] signatureArray)
        {
            var v = signatureArray[64];

            if (v is 0 or 1)
                v = (byte)(v + 27);

            var r = new byte[32];
            Array.Copy(signatureArray, r, 32);
            var s = new byte[32];
            Array.Copy(signatureArray, 32, s, 0, 32);

            return FromComponents(r, s, v);
        }

        public static EthECDSASignature FromComponents(byte[] r, byte[] s) => new EthECDSASignature(new BigInteger(1, r), new BigInteger(1, s));

        public static EthECDSASignature FromComponents(byte[] r, byte[] s, byte v)
        {
            var signature = FromComponents(r, s);
            signature.V = new[] { v };
            return signature;
        }

        public static EthECDSASignature FromComponents(byte[] r, byte[] s, byte[] v)
        {
            var signature = FromComponents(r, s);
            signature.V = v;
            return signature;
        }

        public static EthECDSASignature FromComponents(byte[] rs)
        {
            var r = new byte[32];
            var s = new byte[32];
            Array.Copy(rs, 0, r, 0, 32);
            Array.Copy(rs, 32, s, 0, 32);
            var signature = FromComponents(r, s);
            return signature;
        }

        public static EthECDSASignature ExtractECDSASignature(string signature)
        {
            var signatureArray = signature.HexToByteArray();
            return ExtractECDSASignature(signatureArray);
        }

        public BigInteger R { get; }

        public BigInteger S { get; }

        public byte[] V { get; set; }

        public bool IsLowS => S.CompareTo(SecNamedCurves.GetByName("secp256k1").N.ShiftRight(1)) <= 0;

        public static EthECDSASignature FromByteArray(byte[] sig) => new EthECDSASignature(sig);

        public static bool IsValidDER(byte[] bytes)
        {
            try
            {
                FromByteArray(bytes);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public EthECDSASignature MakeCanonical()
        {
            return !IsLowS ? new EthECDSASignature(R, SecNamedCurves.GetByName("secp256k1").N.Subtract(S)) : this;
        }

        public EthECDSASignature CalculateV(byte[] hash, byte[] uncompressedPublicKey)
        {
            var recId = CalculateRecId(hash, uncompressedPublicKey);
            V = new[] { (byte)(recId + 27) };
            return this;
        }

        public int CalculateRecId(byte[] hash, byte[] uncompressedPublicKey)
        {
            var recId = -1;
            for (var i = 0; i < 4; i++)
            {
                var k = RecoverFromSignature(i, hash);
                if (k is null || !k.SequenceEqual(uncompressedPublicKey)) 
                    continue;
                recId = i;
                break;
            }
            if (recId == -1)
                throw new Exception("Could not construct a recoverable key. This should never happen.");
            return recId;
        }
        
        public byte[] RecoverFromSignature(int recId, byte[] message)
        {
            if (recId < 0)
                throw new ArgumentException("recId should be positive");
            if (R.SignValue < 0)
                throw new ArgumentException("r should be positive");
            if (S.SignValue < 0)
                throw new ArgumentException("s should be positive");
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var curve = SecNamedCurves.GetByName("secp256k1");
            var prime = new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F"));
            var n = curve.N;
            var i = BigInteger.ValueOf((long)recId / 2);
            var x = R.Add(i.Multiply(n));

            if (x.CompareTo(prime) >= 0)
                return null;
            
            var compEnc = X9IntegerConverter.IntegerToBytes(x, 1 + X9IntegerConverter.GetByteLength(curve.Curve));
            compEnc[0] = (byte)((recId & 1) == 1 ? 0x03 : 0x02);
            var r = curve.Curve.DecodePoint(compEnc);

            if (!r.Multiply(n).IsInfinity)
                return null;

            var e = new BigInteger(1, message);
            var eInv = BigInteger.Zero.Subtract(e).Mod(n);
            var rInv = R.ModInverse(n);
            var srInv = rInv.Multiply(S).Mod(n);
            var eInvrInv = rInv.Multiply(eInv).Mod(n);
            var q = ECAlgorithms.SumOfTwoMultiplies(curve.G, eInvrInv, r, srInv);
            q = q.Normalize();
            var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
            return new ECPublicKeyParameters(q, domainParams).ToECPublicKeyByteArray();
            //return curve.Curve.CreatePoint(q.XCoord.ToBigInteger(), q.YCoord.ToBigInteger()).GetEncoded(false);
        }
        
        public int RecoverFromSignature(byte[] message, byte[] uncompressedPublicKey)
        {
            if (R.SignValue < 0)
                throw new ArgumentException("r should be positive");
            if (S.SignValue < 0)
                throw new ArgumentException("s should be positive");
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var curve = SecNamedCurves.GetByName("secp256k1");
            var n = curve.N;
            var e = new BigInteger(1, message);
            var eInv = BigInteger.Zero.Subtract(e).Mod(n);
            var rInv = R.ModInverse(n);
            var srInv = rInv.Multiply(S).Mod(n);
            var eInvrInv = rInv.Multiply(eInv).Mod(n);
            var recId = -1;
            var prime = new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F"));

            for (var i = 0; i < 4; i++)
            {
                recId = i;
                var intAdd = BigInteger.ValueOf((long)recId / 2);
                var x = R.Add(intAdd.Multiply(n));

                if (!(x.CompareTo(prime) >= 0))
                {
                    var compEnc = X9IntegerConverter.IntegerToBytes(x, 1 + X9IntegerConverter.GetByteLength(curve.Curve));
                    compEnc[0] = (byte)((recId & 1) == 1 ? 0x03 : 0x02);
                    var r = curve.Curve.DecodePoint(compEnc);

                    if (r.Multiply(n).IsInfinity)
                    {
                        var q = ECAlgorithms.SumOfTwoMultiplies(curve.G, eInvrInv, r, srInv);
                        q = q.Normalize();
                        if (q.GetEncoded().SequenceEqual(uncompressedPublicKey))
                        {
                            recId = i;
                            break;
                        }
                    }
                }
            }
            if (recId == -1)
                throw new Exception("Could not construct a recoverable key. This should never happen.");
            return recId;
        }


        public byte[] RecoverFromSignature(byte[] hash) => RecoverFromSignature(GetRecIdFromV(V), hash);

        public static int GetRecIdFromV(byte[] v)
        {
            var header = v[0];
            if (header is < 27 or > 34)
                throw new Exception($"Header byte out of range: {header}");
            if (header >= 31)
                header -= 4;
            return header - 27;
        }

        public byte[] ToDER()
        {
            var bos = new MemoryStream(72);
            var seq = new DerSequenceGenerator(bos);
            seq.AddObject(new DerInteger(R));
            seq.AddObject(new DerInteger(S));
            seq.Close();
            return bos.ToArray();
        }

        public byte[] ToByteArray()
        {
            return ToString().HexToByteArray();
        }

        public override string ToString()
        {
            //return $"R = {R.ToHexString().PadLeft(64, '0').EnforceHexPrefix()}," +
            //       $"S = {S.ToHexString().PadLeft(64, '0').EnforceHexPrefix()}," +
            //       $"V = {V?.ToBigIntU()}";
            return $"0x{R.ToHexString().PadLeft(64, '0')}{S.ToHexString().PadLeft(64, '0')}{V.ToHexString()}";
        }
    }
}