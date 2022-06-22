using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using CommonLib.Source.Common.Utils.UtilClasses;

namespace CommonLib.Source.Common.Extensions.Collections
{
    public static class ArrayExtensions
    {
        public static T[] Add<T>(this T[] a, T el)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            var size = a.Length;
            Array.Resize(ref a, size + 1);
            a[^1] = el;
            return a;
        }

        public static T[] Swap<T>(this T[] a, int i, int j)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            (a[j], a[i]) = (a[i], a[j]);
            return a;
        }

        public static bool StartWith(this byte[] data, byte[] versionBytes)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (versionBytes == null)
                throw new ArgumentNullException(nameof(versionBytes));

            if (data.Length < versionBytes.Length)
                return false;
            return !versionBytes.Where((t, i) => data[i] != t).Any();
        }

        public static byte[] SafeSubarray(this byte[] array, int offset, int count)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (offset < 0 || offset > array.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0 || offset + count > array.Length)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (offset == 0 && array.Length == count)
                return array;
            var data = new byte[count];
            Buffer.BlockCopy(array, offset, data, 0, count);
            return data;
        }

        public static byte[] SafeSubarray(this byte[] array, int offset)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (offset < 0 || offset > array.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            var count = array.Length - offset;
            var data = new byte[count];
            Buffer.BlockCopy(array, offset, data, 0, count);
            return data;
        }

        public static byte[] Concat(this byte[] arr, params byte[][] arrs)
        {
            if (arr == null)
                throw new ArgumentNullException(nameof(arr));

            var len = arr.Length + arrs.Sum(a => a.Length);
            var ret = new byte[len];
            Buffer.BlockCopy(arr, 0, ret, 0, arr.Length);
            var pos = arr.Length;
            foreach (var a in arrs)
            {
                Buffer.BlockCopy(a, 0, ret, pos, a.Length);
                pos += a.Length;
            }
            return ret;
        }

        public static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array == null) 
                throw new ArgumentNullException(nameof(array));
            if (action == null) 
                throw new ArgumentNullException(nameof(action));

            if (array.LongLength == 0) 
                return;

            var walker = new ArrayTraverse(array);

            do 
                action(array, walker.Position);
            while (walker.Step());
        }

        public static void Clear<T>(this T[] data)
        {
            if (null != data)
                Array.Clear(data, 0, data.Length);
        }

        public static sbyte[] CopyOf(this sbyte[] data, int newLength)
        {
            var tmp = new sbyte[newLength];
            Array.Copy(data, 0, tmp, 0, System.Math.Min(newLength, data.Length));
            return tmp;
        }

        public static byte[] CompressGZip(this byte[] arr)
        {
            using var outputStream = new MemoryStream();
            using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
                gZipStream.Write(arr, 0, arr.Length);
            return outputStream.ToArray();
        }

        public static byte[] DecompressGZip(this byte[] arr)
        {
            using var inputStream = new MemoryStream(arr);
            using var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();
            gZipStream.CopyTo(outputStream);
            return outputStream.ToArray();
        }
    }
}
