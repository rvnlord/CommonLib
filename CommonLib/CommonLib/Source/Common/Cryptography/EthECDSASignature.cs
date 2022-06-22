using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions;
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
        public static readonly X9ECParameters _secp256k1 = SecNamedCurves.GetByName("secp256k1");
        public static readonly BigInteger HALF_CURVE_ORDER = _secp256k1.N.ShiftRight(1);
        public static readonly BigInteger CURVE_ORDER = _secp256k1.N;
        
        public IReadOnlyList<byte> R { get; }
        public IReadOnlyList<byte> S { get; }
        public IReadOnlyList<byte> V { get; set; }

        public EthECDSASignature(byte[] signature)
        {
            var decoder = new Asn1InputStream(signature);
            if (!(decoder.ReadObject() is DerSequence seq) || seq.Count != 2)
                throw new FormatException("Invalid DER signature");
            decoder.Dispose();

            R = ((DerInteger)seq[0]).Value.ToByteArray();

            var s = ((DerInteger) seq[1]).Value;
            S = (s.CompareTo(HALF_CURVE_ORDER) > 0 ? CURVE_ORDER.Subtract(s) : s).ToByteArray();
        }

        public EthECDSASignature(IReadOnlyList<BigInteger> rs)
        {
            if (rs == null)
                throw new ArgumentNullException(nameof(rs));

            R = rs[0].ToByteArrayUnsigned();
            S = rs[1].ToByteArrayUnsigned();
        }

        public byte[] ToByteArray()
        {
            var bos = new MemoryStream(72);
            var seq = new DerSequenceGenerator(bos);
            seq.AddObject(new DerInteger(R.ToArray()));
            seq.AddObject(new DerInteger(S.ToArray()));
            seq.Close();
            return bos.ToArray();
        }

        public EthECDSASignature CalculateV(AsymmetricKeyParameter pubKey, byte[] hash)
        {
            V = new[] { (byte)(CalculateRecId(pubKey, hash) + 27) };
            return this;
        }

        public int CalculateRecId(AsymmetricKeyParameter pubKey, byte[] hash)
        {
            var recId = -1;
            for (var i = 0; i < 4; i++)
            {
                var k = RecoverKey(i, hash);

                if (k == null || !k.Equals(pubKey))
                    continue;
                recId = i;
                break;
            }

            if (recId < 0) throw new ArgumentException(nameof(recId));
            return recId;
        }

        public AsymmetricKeyParameter RecoverKey(int recId, byte[] hash)
        {
            var curve = _secp256k1;

            var n = curve.N;
            var r = R.ToBigIntU();
            var s = S.ToBigIntU();
            var i = BigInteger.ValueOf((long)recId / 2);
            var x = r.Add(i.Multiply(n));

            var prime = new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F"));
            if (x.CompareTo(prime) >= 0)
                return null;

            var newR = DecompressKey(x, (recId & 1) == 1);

            if (!newR.Multiply(n).IsInfinity)
                return null;

            var e = new BigInteger(1, hash);

            var eInv = BigInteger.Zero.Subtract(e).Mod(n);
            var rInv = r.ModInverse(n);
            var srInv = rInv.Multiply(s).Mod(n);
            var eInvrInv = rInv.Multiply(eInv).Mod(n);
            var q = ECAlgorithms.SumOfTwoMultiplies(curve.G, eInvrInv, newR, srInv);
            q = q.Normalize();

            var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
            return new ECPublicKeyParameters(q, domainParams);
        }

        private static ECPoint DecompressKey(BigInteger xBN, bool yBit)
        {
            var curve = _secp256k1.Curve;
            var compEnc = X9IntegerConverter.IntegerToBytes(xBN, 1 + X9IntegerConverter.GetByteLength(curve));
            compEnc[0] = (byte)(yBit ? 0x03 : 0x02);
            return curve.DecodePoint(compEnc);
        }

        public override string ToString()
        {
            return $"R = {R.ToArray().ToHexString().PadLeft(64, '0').EnforceHexPrefix()}," +
                   $"S = {S.ToArray().ToHexString().PadLeft(64, '0').EnforceHexPrefix()}," +
                   $"V = {V?.ToBigIntU()}";
        }
    }
}