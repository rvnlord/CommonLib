using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using CommonLib.Source.Common.Utils.UtilClasses;

namespace CommonLib.Source.Common.Extensions.Collections
{
    public static class ArrayExtensions
    {
        public static async Task<TSource[]> SetAsync<TSource>(this TSource[] source, int index, TSource el)
        {
            await Task.Run(() => source[index] = el);
            return source;
        }

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

        public static bool StartsWith(this byte[] data, byte[] startsWith)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (startsWith == null)
                throw new ArgumentNullException(nameof(startsWith));

            return data.Length >= startsWith.Length && startsWith.All((t, i) => data[i] == t);
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

        public static async Task<byte[]> ConcatAsync(this byte[] arr, params byte[][] arrs) => await Task.Run(() => arr.Concat(arrs));
        
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

        public static T ValueOrDefault<T>(this T[] arr, int i) => i >= arr.Length ? default : arr[i];
        public static T VorD<T>(this T[] arr, int i) => arr.ValueOrDefault(i);
        public static T ValueOrNull<T>(this T[] arr, int i) where T : class => i >= arr.Length ? null : arr[i];
        public static T VorN<T>(this T[] arr, int i) where T : class => arr.ValueOrNull(i);
    }
}
