using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class ExtendedImage
    {
        private Image _image;
        public IImageFormat Format { get; set; }

        public ExtendedImage(Image image)
        {
            _image = image;
        }

        public static ExtendedImage Load(string physicalPath)
        {
            var extendedImage = new ExtendedImage(Image.Load(physicalPath));
            if (physicalPath.EndsWith(".png"))
                extendedImage.Format = PngFormat.Instance;
            else if (physicalPath.EndsWith(".jpg"))
                extendedImage.Format = JpegFormat.Instance;
            else if (physicalPath.EndsWith(".bmp"))
                extendedImage.Format = BmpFormat.Instance;
            else
                throw new NotSupportedException();

            return extendedImage;
        }

        public string ToBase64DataUrl() => _image.ToBase64String(Format);
    }
}
