using ButtsBlazor.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ButtsBlazor.Api.Model
{
    public static class ButtsDbConfig
    {
        public static IServiceCollection AddButtsDb(this IServiceCollection services)
        {
            services.AddDbContext<ButtsDbContext>(op => op.UseSqlite(ButtsDbContext.DefaultConnectionString), optionsLifetime: ServiceLifetime.Singleton)
                .AddDbContextFactory<ButtsDbContext>(b => b.UseSqlite(ButtsDbContext.DefaultConnectionString));
            return services;
        }


        public static async Task<IServiceProvider> MigrateDatabase(this IServiceProvider @this)
        {
            using var serviceScope = @this.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<ButtsDbContext>();
            var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<ButtsDbContext>>();
            var isNew = !await context.Database.CanConnectAsync() || !File.Exists(ButtsDbContext.DefaultDbPath);
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
