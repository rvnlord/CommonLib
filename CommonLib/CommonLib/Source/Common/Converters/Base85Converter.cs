using System;
using System.IO;
using System.Text;

namespace CommonLib.Source.Common.Converters
{
    public static class Base85Converter
    {
        private const int _asciiOffset = 33;
        private static readonly byte[] _encodedBlock = new byte[5];
        private static readonly byte[] _decodedBlock = new byte[4];
        private static uint _tuple;
        private static readonly uint[] _pow85 = { 85 * 85 * 85 * 85, 85 * 85 * 85, 85 * 85, 85, 1 };

        public static string ToBase85String(this byte[] data)
        {
            var sb = new StringBuilder(data.Length * (_encodedBlock.Length / _decodedBlock.Length));
            var count = 0;
            _tuple = 0;
            foreach (var b in data)
            {
                if (count >= _decodedBlock.Length - 1)
                {
                    _tuple |= b;
                    if (_tuple == 0)
                        sb.Append('z');
                    else
                        EncodeBlock(sb);
                    _tuple = 0;
                    count = 0;
                }
                else
                {
                    _tuple |= (uint)(b << (24 - count * 8));
                    count++;
                }
            }

            if (count > 0)
                EncodeBlock(count + 1, sb);
            return sb.ToString();
        }

        private static void EncodeBlock(StringBuilder sb) => EncodeBlock(_encodedBlock.Length, sb);
        private static void EncodeBlock(int count, StringBuilder sb)
        {
            for (var i = _encodedBlock.Length - 1; i >= 0; i--)
            {
                _encodedBlock[i] = (byte)((_tuple % 85) + _asciiOffset);
                _tuple /= 85;
            }

            for (var i = 0; i < count; i++)
            {
                var c = (char)_encodedBlock[i];
                sb.Append(c);
            }
        }

        public static byte[] Base85ToByteArray(this string data)
        {
            var ms = new MemoryStream();
            var count = 0;

            foreach (var c in data)
            {
                bool processChar;
                switch (c)
                {
                    case 'z':
                        if (count != 0)
                            throw new Exception("The character 'z' is invalid inside an ASCII85 block.");
                        _decodedBlock[0] = 0;
                        _decodedBlock[1] = 0;
                        _decodedBlock[2] = 0;
                        _decodedBlock[3] = 0;
                        ms.Write(_decodedBlock, 0, _decodedBlock.Length);
                        processChar = false;
                        break;
                    case '\n':
                    case '\r':
                    case '\t':
                    case '\0':
                    case '\f':
                    case '\b':
                        processChar = false;
                        break;
                    default:
                        if (c < '!' || c > 'u')
                            throw new Exception("Bad character '" + c + "' found. ASCII85 only allows characters '!' to 'u'.");
                        processChar = true;
                        break;
                }

                if (!processChar)
                    continue;

                _tuple += ((uint)(c - _asciiOffset) * _pow85[count]);
                count++;
                if (count != _encodedBlock.Length)
                    continue;

                DecodeBlock();
                ms.Write(_decodedBlock, 0, _decodedBlock.Length);
                _tuple = 0;
                count = 0;
            }

            switch (count)
            {
                case 0:
                    return ms.ToArray();
                case 1:
                    throw new Exception("The last block of ASCII85 data cannot be a single byte.");
            }

            count--;
            _tuple += _pow85[count];
            DecodeBlock(count);
            for (var i = 0; i < count; i++)
                ms.WriteByte(_decodedBlock[i]);

            return ms.ToArray();
        }

        private static void DecodeBlock() => DecodeBlock(_decodedBlock.Length);
        private static void DecodeBlock(int bytes)
        {
            for (var i = 0; i < bytes; i++)
                _decodedBlock[i] = (byte)(_tuple >> 24 - i * 8);
        }
    }
}
