using System;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions.Collections;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace CommonLib.Source.Common.Cryptography
{
    public class KuznyechikEngine : IBlockCipher // GOST3412_2015
    {
        private static readonly sbyte[] _pi =
        {
            -4, -18, -35, 17, -49, 110, 49, 22, -5, -60, -6, -38, 35, -59, 4, 77, -23, 119, -16, -37, -109, 46, -103, -70,
            23, 54, -15, -69, 20, -51, 95, -63, -7, 24, 101, 90, -30, 92, -17, 33, -127, 28, 60, 66, -117, 1, -114, 79, 5,
            -124, 2, -82, -29, 106, -113, -96, 6, 11, -19, -104, 127, -44, -45, 31, -21, 52, 44, 81, -22, -56, 72, -85, -14,
            42, 104, -94, -3, 58, -50, -52, -75, 112, 14, 86, 8, 12, 118, 18, -65, 114, 19, 71, -100, -73, 93, -121, 21,
            -95, -106, 41, 16, 123, -102, -57, -13, -111, 120, 111, -99, -98, -78, -79, 50, 117, 25, 61, -1, 53, -118, 126,
            109, 84, -58, -128, -61, -67, 13, 87, -33, -11, 36, -87, 62, -88, 67, -55, -41, 121, -42, -10, 124, 34, -71,
            3, -32, 15, -20, -34, 122, -108, -80, -68, -36, -24, 40, 80, 78, 51, 10, 74, -89, -105, 96, 115, 30, 0, 98, 68,
            26, -72, 56, -126, 100, -97, 38, 65, -83, 69, 70, -110, 39, 94, 85, 47, -116, -93, -91, 125, 105, -43, -107,
            59, 7, 88, -77, 64, -122, -84, 29, -9, 48, 55, 107, -28, -120, -39, -25, -119, -31, 27, -125, 73, 76, 63, -8,
            -2, -115, 83, -86, -112, -54, -40, -123, 97, 32, 113, 103, -92, 45, 43, 9, 91, -53, -101, 37, -48, -66, -27,
            108, 82, 89, -90, 116, -46, -26, -12, -76, -64, -47, 102, -81, -62, 57, 75, 99, -74
        };

        private static readonly sbyte[] _inversePI =
        {
            -91, 45, 50, -113, 14, 48, 56, -64, 84, -26, -98, 57, 85, 126, 82, -111, 100, 3, 87, 90, 28, 96, 7, 24, 33, 114,
            -88, -47, 41, -58, -92, 63, -32, 39, -115, 12, -126, -22, -82, -76, -102, 99, 73, -27, 66, -28, 21, -73, -56, 6,
            112, -99, 65, 117, 25, -55, -86, -4, 77, -65, 42, 115, -124, -43, -61, -81, 43, -122, -89, -79, -78, 91, 70, -45,
            -97, -3, -44, 15, -100, 47, -101, 67, -17, -39, 121, -74, 83, 127, -63, -16, 35, -25, 37, 94, -75, 30, -94, -33,
            -90, -2, -84, 34, -7, -30, 74, -68, 53, -54, -18, 120, 5, 107, 81, -31, 89, -93, -14, 113, 86, 17, 106, -119,
            -108, 101, -116, -69, 119, 60, 123, 40, -85, -46, 49, -34, -60, 95, -52, -49, 118, 44, -72, -40, 46, 54, -37,
            105, -77, 20, -107, -66, 98, -95, 59, 22, 102, -23, 92, 108, 109, -83, 55, 97, 75, -71, -29, -70, -15, -96, -123,
            -125, -38, 71, -59, -80, 51, -6, -106, 111, 110, -62, -10, 80, -1, 93, -87, -114, 23, 27, -105, 125, -20, 88, -9,
            31, -5, 124, 9, 13, 122, 103, 69, -121, -36, -24, 79, 29, 78, 4, -21, -8, -13, 62, 61, -67, -118, -120, -35, -51,
            11, 19, -104, 2, -109, -128, -112, -48, 36, 52, -53, -19, -12, -50, -103, 16, 68, 64, -110, 58, 1, 38, 18, 26,
            72, 104, -11, -127, -117, -57, -42, 32, 10, 8, 0, 76, -41, 116
        };

        private readonly sbyte[] _lFactors = { -108, 32, -123, 16, -62, -64, 1, -5, 1, -64, -62, 16, -123, 32, -108, 1 };
        private const int _keyLength = 32;
        private const int _blockSize = 16;
        private static int _subLength => _keyLength / 2;
        private sbyte[][] _subKeys;
        private bool _forEncryption;
        private readonly sbyte[][] _gf256MulTable = GetGf256MulTable();

        public string AlgorithmName => "Kuznyechik";
        public static int BlockSize => _blockSize;
        public bool IsPartialBlockOkay => false;

        public int GetBlockSize() => _blockSize;

        private static sbyte[][] GetGf256MulTable()
        {
            var mulTable = new sbyte[256][];
            for (var x = 0; x < 256; x++)
            {
                mulTable[x] = new sbyte[256];
                for (var y = 0; y < 256; y++)
                    mulTable[x][y] = KuzMulGf256Slow((sbyte)x, (sbyte)y);
            }
                
            return mulTable;
        }

        private static sbyte KuzMulGf256Slow(sbyte a, sbyte b)
        {
            sbyte p = 0;
            sbyte counter;
            for (counter = 0; counter < 8 && a != 0 && b != 0; counter++)
            {
                if ((b & 1) != 0)
                    p ^= a;
                var hiBitSet = (sbyte)(a & 0x80);
                a <<= 1;
                if (hiBitSet != 0)
                    a ^= unchecked((sbyte)0xc3); /* x^8 + x^7 + x^6 + x + 1 */
                b >>= 1;
            }

            return p;
        }

        public void Init(bool forEncryption, ICipherParameters parameters)
        {
            if (parameters is KeyParameter keyParam)
            {
                _forEncryption = forEncryption;
                GenerateSubKeys(keyParam.GetKey().ToSigned());
            }
            else if (parameters != null)
                throw new ArgumentException("invalid parameter passed to Kuznyechik init - " + parameters.GetType().Name);
        }

        private void GenerateSubKeys(sbyte[] userKey)
        {
            if (userKey.Length != _keyLength)
                throw new ArgumentException("Key length invalid. Key needs to be 32 byte - 256 bit");

            _subKeys = new sbyte[10][];
            for (var i = 0; i < 10; i++)
                _subKeys[i] = new sbyte[_subLength];

            var x = new sbyte[_subLength];
            var y = new sbyte[_subLength];

            for (var i = 0; i < _subLength; i++)
            {
                _subKeys[0][i] = x[i] = userKey[i];
                _subKeys[1][i] = y[i] = userKey[i + _subLength];
            }

            var c = new sbyte[_subLength];

            for (var k = 1; k < 5; k++)
            {
                for (var j = 1; j <= 8; j++)
                {
                    C(c, 8 * (k - 1) + j);
                    F(c, x, y);
                }

                Array.Copy(x, 0, _subKeys[2 * k], 0, _subLength);
                Array.Copy(y, 0, _subKeys[2 * k + 1], 0, _subLength);
            }
        }


        private void C(sbyte[] c, int i)
        {
            c.Clear();
            c[15] = (sbyte)i;
            L(c);
        }
        
        private void F(sbyte[] k, sbyte[] a1, sbyte[] a0)
        {
            var temp = LSX(k, a1);
            X(temp, a0);
            Array.Copy(a1, 0, a0, 0, _subLength);
            Array.Copy(temp, 0, a1, 0, _subLength);
        }

        public int ProcessBlock(sbyte[] input, int inOff, sbyte[] output, int outOff)
        {
            if (_subKeys == null)
                throw new Exception("Kuznyechik engine not initialised");
            if (inOff + _blockSize > input.Length)
                throw new DataLengthException("input buffer too short");
            if (outOff + _blockSize > output.Length)
                throw new OutputLengthException("output buffer too short");

            KuznyechikFunc(input, inOff, output, outOff);
            return _blockSize;
        }

        public int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff) => ProcessBlock(input.ToSigned(), inOff, output.ToSigned(), outOff);

        private void KuznyechikFunc(sbyte[] input, int inOff, sbyte[] output, int outOff)
        {
            var block = new sbyte[_blockSize];
            Array.Copy(input, inOff, block, 0, _blockSize);

            if (_forEncryption)
            {
                for (var i = 0; i < 9; i++)
                {

                    var temp = LSX(_subKeys[i], block);
                    block = temp.CopyOf(_blockSize);
                }

                X(block, _subKeys[9]);
            }
            else
            {
                for (var i = 9; i > 0; i--)
                {
                    var temp = XSL(_subKeys[i], block);
                    block = temp.CopyOf(_blockSize);
                }

                X(block, _subKeys[0]);
            }

            Array.Copy(block, 0, output, outOff, _blockSize);
        }

        private sbyte[] LSX(sbyte[] k, sbyte[] a)
        {
            var result = k.CopyOf(k.Length);
            X(result, a);
            S(result);
            L(result);
            return result;
        }

        private sbyte[] XSL(sbyte[] k, sbyte[] a)
        {
            var result = k.CopyOf(k.Length);
            X(result, a);
            InverseL(result);
            InverseS(result);
            return result;
        }

        private static void X(sbyte[] result, sbyte[] data)
        {
            for (var i = 0; i < result.Length; i++)
                result[i] ^= data[i];
        }

        private static void S(sbyte[] data)
        {
            for (var i = 0; i < data.Length; i++)
                data[i] = _pi[UnsignedByte(data[i])];
        }

        private static void InverseS(sbyte[] data)
        {
            for (var i = 0; i < data.Length; i++)
                data[i] = _inversePI[UnsignedByte(data[i])];
        }

        private static int UnsignedByte(sbyte b) => b & 0xFF;

        private void L(sbyte[] data)
        {
            for (var i = 0; i < 16; i++)
                R(data);
        }

        private void InverseL(sbyte[] data)
        {
            for (var i = 0; i < 16; i++)
                InverseR(data);
        }


        private void R(sbyte[] data)
        {
            var z = l(data);
            Array.Copy(data, 0, data, 1, 15);
            data[0] = z;
        }

        private void InverseR(sbyte[] data)
        {
            var temp = new sbyte[16];
            Array.Copy(data, 1, temp, 0, 15);
            temp[15] = data[0];
            var z = l(temp);
            Array.Copy(data, 1, data, 0, 15);
            data[15] = z;
        }


        private sbyte l(sbyte[] data)
        {
            var x = data[15];
            for (var i = 14; i >= 0; i--)
                x ^= _gf256MulTable[UnsignedByte(data[i])][UnsignedByte(_lFactors[i])];
            return x;
        }
        
        public void Reset() { }
    }
}
