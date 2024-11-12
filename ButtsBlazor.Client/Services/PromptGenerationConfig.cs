using ButtsBlazor.Client.Utils;
using ButtsBlazor.Shared.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using PubSub;

namespace ButtsBlazor.Client.Services;

public static class PromptGenerationConfig
{
    public static IServiceCollection AddClientButts(this IServiceCollection @this)
    {
        @this.AddTransient(s => s.GetRequiredService<IOptions<PromptOptions>>().Value);
        @this.AddTransient(s => s.GetRequiredService<PromptOptions>().GenerationOptions);
        @this.AddSingleton<ButtsOptionsProvider>();
        @this.AddSingleton(p => (IOptions<PromptOptions>)p.GetRequiredService<ButtsOptionsProvider>());
        @this.AddSingleton<Hub>();
        @this.AddSingleton<ButtsHubManager>();
        @this.AddTransient<IButtsNotificationClient, ButtsNotificationClient>();
        @this.AddScoped<Random>();
        @this.AddScoped<IPromptGenerationService,SnickPromptGenerationService>();
        @this.AddSingleton<History>();
        @this.AddScoped<Func<HttpClient>>(sp =>
        {
            var f = sp.GetRequiredService<IHttpClientFactory>();
                return () => f.CreateClient();
        });
        @this.AddScoped<IButtsApiClient, ButtsApiClient>();

        return @this;
    }

    public static async Task<WebAssemblyHost> UseButts(this WebAssemblyHost @this)
    {
        var config = @this.Services.GetRequiredService<ButtsOptionsProvider>();
        await config.GetOptions();
        return @this;

    }
}