using System.Collections.Generic;
using CommonLib.Source.Common.Utils.UtilClasses;

namespace CommonLib.Source.Common.Converters
{
    public static class ListConverter
    {
        public static FileDataList ToFileDataList(this List<FileData> list) => new(list);
    }
}
