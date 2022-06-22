using System;
using System.Linq;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions;
using CommonLib.Source.Common.Extensions.Collections;
using CommonLib.Source.Common.Utils.TypeUtils;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public struct FileSize
    {
        private static FileSizeSuffix[] _suffixes => EnumUtils.GetValues<FileSizeSuffix>().ToArray();

        public long SizeInBytes { get; }
        public FileSizeSuffix Suffix { get; }
        public double Size => GetSize();

        public FileSize(long sizeInBytes)
        {
            var i = 0;  
            var number = (double)sizeInBytes;  
            while (Math.Round(number / 1024) >= 1)  
            {  
                number /= 1024;  
                i++;  
            }

            SizeInBytes = sizeInBytes;
            Suffix = _suffixes[i];
        }

        public FileSize(double size, FileSizeSuffix suffix)
        {
            var i = _suffixes.IndexOf_(suffix);
            SizeInBytes = (size * Math.Pow(1024, i)).Round().ToLong();
            Suffix = suffix;
        }
        
        private double GetSize()
        {
            var i = _suffixes.IndexOf_(Suffix);
            return (SizeInBytes / Math.Pow(1024, i)).Round(2);
        }

        public override string ToString() => $"{Size} {Suffix}";
    }

    public enum FileSizeSuffix
    {
        B,
        KB,
        MB,
        GB,
        TB,
        PB
    }
}
