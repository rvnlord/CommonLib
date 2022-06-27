using System;
using System.IO;
using CommonLib.Source.Common.Utils;

namespace CommonLib.Source.Common.Extensions
{
    public static class FileInfoExtensions
    {
        public static void Rename(this FileInfo fileInfo, string newName)
        {
            fileInfo.MoveTo(PathUtils.Combine(PathSeparator.BSlash, fileInfo.Directory?.FullName ?? throw new NullReferenceException(), newName));
        }
    }
}
