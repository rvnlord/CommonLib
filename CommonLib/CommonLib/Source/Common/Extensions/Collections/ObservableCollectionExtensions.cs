using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommonLib.Source.Common.Converters;

namespace CommonLib.Source.Common.Extensions.Collections
{
    public static class ObservableCollectionExtensions
    {
        public static ObservableCollection<T> ReplaceAll<T>(this ObservableCollection<T> obsCol, IEnumerable<T> newEnumerable)
        {
            var list = newEnumerable.ToList();
            obsCol.RemoveAll();
            obsCol.AddRange(list);
            return obsCol;
        }

        public static ObservableCollection<T> ReplaceAll<T>(this ObservableCollection<T> obsCol, T newEl)
        {
            return obsCol.ReplaceAll(newEl.ToEnumerable());
        }
    }
}
