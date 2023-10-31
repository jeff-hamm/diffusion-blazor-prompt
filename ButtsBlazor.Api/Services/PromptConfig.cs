using ButtsBlazor.Api.Model;
using ButtsBlazor.Client.Services;
using ButtsBlazor.Client.Utils;
using ButtsBlazor.Client.ViewModels;
using ButtsBlazor.Services;
using ButtsBlazor.Shared.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PubSub;

namespace ButtsBlazor.Api.Services;

public static class PromptConfig
{
        
    public static IServiceCollection AddButts(this IServiceCollection @this, IConfigurationManager configurationManager)
    {
        @this.AddSingleton<ButtsListFileService>();
        @this.AddSingleton<FileService>();
        @this.AddSingleton<ImagePathService>();
        @this.Configure<PromptOptions>(configurationManager.GetSection(nameof(PromptOptions)));
        @this.AddSingleton(sp => sp.GetService<IOptions<PromptOptions>>()?.Value ?? throw new InvalidOperationException($"Could not find PromptOptions"));
        @this.AddSingleton<PromptQueue>();

        @this.AddTransient<IButtsNotificationClient, ServerNotificationClient>();
        @this.AddScoped<Random>();
        @this.AddScoped<PromptGenerationService>();
        @this.AddScoped<IButtsApiClient, FakeButtsApiClient>();

        return @this;
    }
}

public class FakeButtsApiClient : IButtsApiClient
{
    public Task<UploadResult> UploadFile(string dataUrlString)
    {
        return Task.FromResult<UploadResult>(new UploadResult());
    }

    public Task<UploadResult> UploadFile(IBrowserFile file)
    {
        return Task.FromResult<UploadResult>(new UploadResult());
    }

    public Task<WebPath[]> GetRecentImages(int numImages)
    {
        return Task.FromResult(Array.Empty<WebPath>());
    }
}