using System.Collections.Generic;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class FileData
    {
        public long? TotalSizeInBytes { get; set; }
        public FileSize TotalSize => new (TotalSizeInBytes ?? 0);
        public FileSize ChunkSize => new (Data.Count);
        public string Path => PathUtils.Combine(PathSeparator.BSlash, DirectoryPath, Name, ".", Extension);
        public List<byte> Data { get; set; }
        public long Position { get; set; }
        public double Progress => (double)Position / (TotalSizeInBytes ?? ChunkSize.SizeInBytes) * 100;
        public string Name { get; set; }
        public string Extension { get; set; }
        public string DirectoryPath { get; set; }
        public bool IsSelected { get; set; }
        public UploadStatus Status { get; set; }
        public string NameExtensionAndSize => $"{Name}.{Extension} ({TotalSize})";
    }

    public enum UploadStatus
    {
        NotStarted,
        Uploading,
        Paused,
        Finished,
        Failed
    }
}
