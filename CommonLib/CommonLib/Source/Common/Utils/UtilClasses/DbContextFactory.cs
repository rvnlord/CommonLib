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
        public static TDbContext CreateSQLite<TDbContext>() where TDbContext : DbContext, new() => new DefaultDbContextFactory<TDbContext>().CreateDbContext(Array.Empty<string>());

        public static DbContextOptionsBuilder<TContext> GetSQLiteDbContextOptionsBuilder<TContext>(string csConfigName = "DBCS", string migrationAssembly = null, DbContextOptionsBuilder<TContext> b = null) where TContext : Microsoft.EntityFrameworkCore.DbContext
        {
            // DBCS: "Data Source=.\\Database\\store.db"
            var dbcs = ConfigUtils.GetFromAppSettings().GetConnectionString(csConfigName);
            var dbPath = PathUtils.Combine(PathSeparator.BSlash, FileUtils.GetEntryAssemblyDir(), dbcs.AfterFirst("="));
            dbcs = $"{dbcs.BeforeFirst("=")}={dbPath}";
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath) ?? throw new NullReferenceException()); // SQLite provider has no access to creating folders so migration would crash

            return (b ?? new DbContextOptionsBuilder<TContext>()).UseSqlite(dbcs, o => o.MigrationsAssembly(migrationAssembly ?? typeof(TContext).Assembly.FullName)).EnableSensitiveDataLogging();
        }

        public static DbContextOptions<TContext> GetSQLiteDbContextOptions<TContext>(string csConfigName = "DBCS", string migrationAssembly = null, DbContextOptionsBuilder<TContext> b = null) where TContext : Microsoft.EntityFrameworkCore.DbContext
            => GetSQLiteDbContextOptionsBuilder<TContext>(csConfigName, migrationAssembly, b).Options;
        
        public static DbContextOptionsBuilder<TContext> GetMSSQLDbContextBuilder<TContext>(string csConfigName = "DBCS", string migrationAssembly = null, DbContextOptionsBuilder<TContext> b = null) where TContext : Microsoft.EntityFrameworkCore.DbContext
        {
            return (b ?? new DbContextOptionsBuilder<TContext>()).UseSqlServer(ConfigUtils.GetFromAppSettings().GetConnectionString(csConfigName), o => o.MigrationsAssembly(migrationAssembly ?? typeof(TContext).Assembly.FullName)).EnableSensitiveDataLogging();
        }

        public static DbContextOptions<TContext> GetMSSQLDbContextOptions<TContext>(string csConfigName = "DBCS", string migrationAssembly = null, DbContextOptionsBuilder<TContext> b = null) where TContext : Microsoft.EntityFrameworkCore.DbContext
            => GetMSSQLDbContextBuilder<TContext>(csConfigName, migrationAssembly, b).Options;

        public class DefaultDbContextFactory<TDbContext> : IDesignTimeDbContextFactory<TDbContext> where TDbContext : DbContext, new()
        { // this is called by Migrations, in turn it calls parameterless constructor which uses `base()` with the result of `GetSQLiteDbContextOptions()` as parameter
            public TDbContext CreateDbContext(string[] args) => new();
        }
    }

}
