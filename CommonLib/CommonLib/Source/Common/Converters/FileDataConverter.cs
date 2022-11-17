using System;
using System.IO;
using System.Linq;
using CommonLib.Source.Common.Extensions;
using CommonLib.Source.Common.Utils;
using CommonLib.Source.Common.Utils.UtilClasses;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace CommonLib.Source.Common.Converters
{
    public static class FileDataConverter
    {
        private static readonly string[] _imgExtensions = { "jpg", "jpeg", "jpe", "jif", "jfif", "jfi", "png", "bmp", "gif", "ico", "webp", "svg" };

        public static FileData ToFileData(this FileInfo fi, bool loadFile = false)
        {
            return new FileData
            {
                CreationTime = fi.CreationTimeUtc.ToExtendedTime(),
                TotalSizeInBytes = fi.Length,
                Data = loadFile ? FileUtils.ReadBytes(fi.FullName, 0, (int)fi.Length).ToList() : null,
                Position = 0,
                Name = fi.FullName.PathToName(),
                Extension = fi.FullName.PathToExtension(),
                DirectoryPath = fi.DirectoryName,
                IsSelected = false,
                Status = UploadStatus.NotStarted,
                IsPreAdded = false,
                IsExtensionValid = false,
                IsFileSizeValid = false,
                ValidateUploadStatus = false
            };
        }

        public static string ToBase64ImageString(this FileData fd)
        {
            if (!fd.Extension.In(_imgExtensions))
                throw new FormatException("This is not an image file");

            var format = fd.Extension.In("jpg", "jpeg", "jpe", "jif", "jfif", "jfi") ? "jpeg" : fd.Extension;

            return $"data:image/{format};base64,{fd.Data.ToBase64String()}";
        }

        public static bool IsBase64ImageString(this string base64Image) => base64Image.IsBase64() && base64Image.ToLowerInvariant().BetweenOrNull("data:image/", ";base64,")?.In(_imgExtensions) == true;

        public static FileData Base64ImageStringToFileData(this string base64Image)
        {
            if (!base64Image.IsBase64ImageString())
                throw new FormatException("This is not a vaslid base64 image string");

            var data = base64Image.ToLowerInvariant().After(";base64,").Base64ToByteArray();
            var extension = base64Image.ToLowerInvariant().Between("data:image/", ";base64,");

            return new FileData
            {
                TotalSizeInBytes = data.Length,
                Data = data.ToList(),
                Position = 0,
                Name = null,
                Extension = extension,
                DirectoryPath = null,
                CreationTime = null,
                IsSelected = false,
                Status = UploadStatus.NotStarted,
                IsPreAdded = false,
                IsExtensionValid = false,
                IsFileSizeValid = false,
                ValidateUploadStatus = false
            };
        }

        public static FileData PathToFileData(this string filePath, bool loadFile) => new FileInfo(filePath).ToFileData(loadFile);

        public static ExtendedImage ToExtendedImage(this FileData fd) => new(Image.Load(fd.Data.ToArray()), fd.Extension);

        public static IImageFormat ExtensionToImageFormat(this string extension)
        {
            IImageFormat format;
            extension = extension.TrimStart('.').ToLowerInvariant();
            if (extension.EqualsInvariant("png"))
                format = PngFormat.Instance;
            else if (extension.EqualsInvariant("jpg"))
                format = JpegFormat.Instance;
            else if (extension.EqualsInvariant("bmp"))
                format = BmpFormat.Instance;
            else
                throw new NotSupportedException();

            return format;
        }
    }
}
