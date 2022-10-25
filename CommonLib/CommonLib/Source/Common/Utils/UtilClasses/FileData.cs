using System;
using System.Collections.Generic;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class FileData : IEquatable<FileData>
    {
        public long? TotalSizeInBytes { get; set; }
        public FileSize TotalSize => new (TotalSizeInBytes ?? 0);
        public FileSize ChunkSize => new (Data.Count);
        public string Path => PathUtils.Combine(PathSeparator.BSlash, DirectoryPath, Name, ".", Extension);
        public List<byte> Data { get; set; } = new();
        public long Position { get; set; }
        public double Progress => (double)Position / (TotalSizeInBytes ?? ChunkSize.SizeInBytes) * 100;
        public string Name { get; set; }
        public string Extension { get; set; }
        public string DirectoryPath { get; set; }
        public bool IsSelected { get; set; }
        public UploadStatus Status { get; set; }
        public string NameWithExtension => $"{Name}.{Extension}";
        public string NameExtensionAndSize => $"{NameWithExtension} ({TotalSize})";

        public bool Equals(FileData other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return TotalSizeInBytes == other.TotalSizeInBytes && string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase) && string.Equals(Extension, other.Extension, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FileData)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(TotalSizeInBytes);
            hashCode.Add(Name, StringComparer.InvariantCultureIgnoreCase);
            hashCode.Add(Extension, StringComparer.InvariantCultureIgnoreCase);
            return hashCode.ToHashCode();
        }

        public static bool operator ==(FileData left, FileData right) => Equals(left, right);
        public static bool operator !=(FileData left, FileData right) => !Equals(left, right);
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
