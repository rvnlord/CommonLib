using System.Collections.Generic;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class FileDataList : CustomList<FileData>
    {
        public FileDataList Self => this;

        public FileDataList(bool isReadOnly = false) : base(isReadOnly) { }
        public FileDataList(List<FileData> list, bool isReadOnly = false) : base(list, isReadOnly) { }
    }
}
