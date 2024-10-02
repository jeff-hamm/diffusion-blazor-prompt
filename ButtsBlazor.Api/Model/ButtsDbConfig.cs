using ButtsBlazor.Api.Services;
using Configuration.EFCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ButtsBlazor.Api.Model
{
    public static class ButtsDbConfig
    {
        public static WebApplicationBuilder AddButtsDb(this WebApplicationBuilder @this, string dbPath)
        {
            var connectionString =
                new SqliteConnectionStringBuilder()
                {
                    DataSource = dbPath,
                    Mode = SqliteOpenMode.ReadWriteCreate,
                }.ToString();
            @this.Services.AddDbContext<ButtsDbContext>(op => 
                op.UseSqlite(connectionString), optionsLifetime: ServiceLifetime.Singleton)
                .AddDbContextFactory<ButtsDbContext>(b => b.UseSqlite());
            @this.Configuration.AddEFCoreConfiguration<ButtsDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            }, reloadOnChange: true, onLoadException: ex => ex.Ignore = true);

            return @this;
        }


        public static async Task<IServiceProvider> MigrateDatabase(this IServiceProvider @this)
        {
            using var serviceScope = @this.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<ButtsDbContext>();
            var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<ButtsDbContext>>();
            var siteConfig = serviceScope.ServiceProvider.GetRequiredService<SiteConfigOptions>();
            var isNew = !await context.Database.CanConnectAsync();
            if (!File.Exists(siteConfig.FullDbPath))
            {
                if(!Directory.Exists(Path.GetDirectoryName(siteConfig.FullDbPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(siteConfig.FullDbPath) ?? string.Empty);
                File.Create(siteConfig.FullDbPath,4096, FileOptions.None);
                isNew = true;
            }
            await context.Database.MigrateAsync();
            if (isNew || !context.Images.Any())
            {
                logger.LogInformation("New DB Detected, starting file scans");
                var fileScanner = serviceScope.ServiceProvider.GetRequiredService<FileService>();
                await fileScanner.FileScan();
            }
            return @this;
        }
    }
}
