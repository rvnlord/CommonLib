using System;
using System.Collections.Generic;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class FileData : IEquatable<FileData>
    {
        private UploadStatus _status;
        private bool _isSelected;
        private bool _prevIsSelected;
        private UploadStatus _prevStatus;

        public long? TotalSizeInBytes { get; set; }
        public FileSize TotalSize => new (TotalSizeInBytes ?? 0);
        public FileSize ChunkSize => new (Data.Count);
        public string Path => PathUtils.Combine(PathSeparator.BSlash, DirectoryPath, Name, ".", Extension);
        public List<byte> Data { get; set; } = new();
        public long Position { get; set; }
        public double Progress
        {
            get
            {
                var totalSize = TotalSizeInBytes ?? ChunkSize.SizeInBytes;
                if (totalSize == 0)
                    return 0;
                return (double)Position / totalSize * 100;
            }
        }

        public string Name { get; set; }
        public string Extension { get; set; }
        public string DirectoryPath { get; set; }
        public ExtendedTime CreationTime { get; set; } = ExtendedTime.UtcNow;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _prevIsSelected = _isSelected;
                _isSelected = value;
                if (_isSelected != _prevIsSelected)
                    OnStateChanging(_prevIsSelected, _isSelected);
            }
        }

        public UploadStatus Status
        {
            get => _status;
            set
            {
                _prevStatus = _status;
                _status = value;
                if (_status != _prevStatus)
                    OnStateChanging(_prevStatus, _status);
            }
        }

        public bool IsPreAdded { get; set; }
        public bool IsExtensionValid { get; set; }
        public bool IsFileSizeValid { get; set; }
        public bool ValidateUploadStatus { get; set; } = true;
        public bool IsValid => IsFileSizeValid && IsExtensionValid && (!ValidateUploadStatus || Status == UploadStatus.Finished);
        public string NameWithExtension => $"{Name}.{Extension}";
        public string NameExtensionAndSize => $"{NameWithExtension} ({TotalSize})";
        public static FileData Empty => new();
        public FileData Self => this;
        public string ChunkHash => Data.Keccak256().ToHexString();
        public string Hash => Data.Count == TotalSizeInBytes ? ChunkHash : null;

        public event MyEventHandler<FileData, FileDataStateChangedEventArgs> StateChanged;
        protected void OnStateChanging(FileDataStateChangedEventArgs e) => StateChanged?.Invoke(this, e);
        protected void OnStateChanging(StatePropertyKind property, OldAndNewValue<bool> isSelected, OldAndNewValue<UploadStatus> status) => OnStateChanging(new FileDataStateChangedEventArgs(property, isSelected, status));
        protected void OnStateChanging(StatePropertyKind property, bool oldIsSelected, bool newIsSelected, UploadStatus oldStatus, UploadStatus newStatus) => OnStateChanging(new FileDataStateChangedEventArgs(property, new OldAndNewValue<bool>(oldIsSelected, newIsSelected), new OldAndNewValue<UploadStatus>(oldStatus, newStatus)));
        protected void OnStateChanging(bool oldIsSelected, bool newIsSelected) => OnStateChanging(new FileDataStateChangedEventArgs(StatePropertyKind.IsSelected, new OldAndNewValue<bool>(oldIsSelected, newIsSelected), new OldAndNewValue<UploadStatus>(Status, Status)));
        protected void OnStateChanging(UploadStatus oldStatus, UploadStatus newStatus) => OnStateChanging(new FileDataStateChangedEventArgs(StatePropertyKind.Status, new OldAndNewValue<bool>(IsSelected, IsSelected), new OldAndNewValue<UploadStatus>(oldStatus, newStatus)));

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

    public class FileDataStateChangedEventArgs : EventArgs
    {
        public StatePropertyKind Property { get; }
        public OldAndNewValue<bool> IsSelected { get; }
        public OldAndNewValue<UploadStatus> Status { get; }

        public FileDataStateChangedEventArgs(StatePropertyKind property, OldAndNewValue<bool> isSelected, OldAndNewValue<UploadStatus> status)
        {
            Property = property;
            IsSelected = isSelected;
            Status = status;
        }
    }
    
    public enum UploadStatus
    {
        NotStarted,
        Uploading,
        Paused,
        Finished,
        Failed
    }

    public enum StatePropertyKind
    {
        IsSelected,
        Status
    }
}
