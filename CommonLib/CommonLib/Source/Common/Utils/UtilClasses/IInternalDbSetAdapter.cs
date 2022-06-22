using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Internal;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    internal interface IInternalDbSetAdapter<TEntity> where TEntity : class
    {
        [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "<Pending>")]
        InternalDbSet<TEntity> InternalSet { get; }
    }
}
