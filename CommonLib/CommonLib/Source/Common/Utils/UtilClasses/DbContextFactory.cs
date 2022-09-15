using System;
using System.Configuration;
using System.IO;
using CommonLib.Source.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public static class DbContextFactory
    {
        public static TDbContext CreateSQLite<TDbContext>() where TDbContext : DbContext, new() => new SQLiteDbContextFactory<TDbContext>().CreateDbContext(Array.Empty<string>());

        public static DbContextOptions<TContext> GetSQLiteDbContextOptions<TContext>(string csConfigName = "DBCS") where TContext : Microsoft.EntityFrameworkCore.DbContext
        {
            // DBCS: "Data Source=.\\Database\\store.db"
            var dbcs = ConfigUtils.GetFromAppSettings().GetConnectionString(csConfigName);
            var dbPath = PathUtils.Combine(PathSeparator.BSlash, FileUtils.GetEntryAssemblyDir(), dbcs.AfterFirst("="));
            dbcs = $"{dbcs.BeforeFirst("=")}={dbPath}";
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath) ?? throw new NullReferenceException()); // SQLite provider has no access to creating folders so migration would crash

            return new DbContextOptionsBuilder<TContext>().UseSqlite(dbcs).Options;
        }

        public class SQLiteDbContextFactory<TDbContext> : IDesignTimeDbContextFactory<TDbContext> where TDbContext : DbContext, new()
        { // this is called by Migrations, in turn it calls parameterless constructor which uses `base()` with the result of `GetSQLiteDbContextOptions()` as parameter
            public TDbContext CreateDbContext(string[] args) => new();
        }
    }

}
