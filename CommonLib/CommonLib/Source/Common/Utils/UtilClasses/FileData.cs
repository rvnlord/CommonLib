using System.Collections.Generic;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class FileData
    {
        public FileSize? TotalSize { get; set; }
        public FileSize ChunkSize => new (Data.Count);
        public string Path { get; set; }
        public List<byte> Data { get; set; }
        public long Position { get; set; }
        public double Progress => (double)Position / (TotalSize?.SizeInBytes ?? ChunkSize.SizeInBytes) * 100;
        public string Name => Path?.AfterLast(@"\");
        public bool IsSelected { get; set; }
    }
}
