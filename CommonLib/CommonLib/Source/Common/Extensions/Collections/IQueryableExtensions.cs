using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CommonLib.Source.Common.Extensions.Collections
{
    public static class IQueryableExtensions
    {
        public static async Task<IQueryable<T>> WhereAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> selector)
        {
            return await Task.Run(() => source.Where(selector));
        }
    }
}
