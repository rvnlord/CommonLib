using CommonLib.Source.Common.Utils.UtilClasses;

namespace CommonLib.Source.Common.Converters
{
    public static class FileSizeConverter
    {
        public static FileSize ToFileSize(this long l) => new(l);
        public static string ToFileSizeString(this long l) => ToFileSize(l).ToString();
    }
}
