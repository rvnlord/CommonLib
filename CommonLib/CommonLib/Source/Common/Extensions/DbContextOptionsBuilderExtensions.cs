using CommonLib.Source.Common.Utils.UtilClasses;
using Microsoft.EntityFrameworkCore;

namespace CommonLib.Source.Common.Extensions
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder<TContext> SQLiteFromAppConfig<TContext>(this DbContextOptionsBuilder<TContext> b, string csConfigName, string migrationAssembly = null) where TContext : DbContext
        {
            return DbContextFactory.GetSQLiteDbContextOptionsBuilder(csConfigName, migrationAssembly, b);
        }
    }
}
