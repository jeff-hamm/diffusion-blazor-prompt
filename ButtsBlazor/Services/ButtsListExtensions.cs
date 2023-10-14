using ButtsBlazor.Api.Model;
using Microsoft.EntityFrameworkCore;

namespace ButtsBlazor.Server.Services;

public static class ButtsListExtensions
{

    public static async Task<WebApplication> MigrateDatabase(this WebApplication @this)
    {
        using var serviceScope = @this.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<ButtsDbContext>();
        await context.Database.MigrateAsync();
        return @this;
    }

    public static IServiceCollection AddButtsList(this IServiceCollection @this)
    {
        @this.AddSingleton<ButtsListFileService>();
        return @this;
    }
}