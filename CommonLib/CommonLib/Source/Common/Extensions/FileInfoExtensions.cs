using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommonLib.Source.Common.Utils;

namespace CommonLib.Source.Common.Extensions
{
    public static class FileInfoExtensions
    {
        public static void Rename(this FileInfo fileInfo, string newName)
        {
            fileInfo.MoveTo(PathUtils.Combine(PathSeparator.BSlash, fileInfo.Directory?.FullName ?? throw new NullReferenceException(), newName), true);
        }

        public static async Task RenameAsync(this FileInfo fileInfo, string newName) => await Task.Run(() => fileInfo.Rename(newName));

        public static async Task DeleteAsync(this FileInfo fileInfo) => await Task.Run(fileInfo.Delete);

        public static void DeleteAll(this IEnumerable<FileInfo> fileInfos)
        {
            foreach (var fi in fileInfos)
                fi.Delete();
        }

        public static async Task DeleteAllAsync(this IEnumerable<FileInfo> fileInfos) => await Task.Run(fileInfos.DeleteAll);
    }
}
