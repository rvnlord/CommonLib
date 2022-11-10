using CommonLib.Source.Common.Converters;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

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
        
        public ExtendedImage(Image image, IImageFormat format)
        {
            _image = image;
            Format = format;
        }

        public ExtendedImage(Image image, string extension)
        {
            _image = image;
            Format = extension.ExtensionToImageFormat();
        }

        public static ExtendedImage Load(string physicalPath) => new(Image.Load(physicalPath), physicalPath.PathToExtension().ExtensionToImageFormat());

        public string ToBase64DataUrl() => _image.ToBase64String(Format);
    }
}
