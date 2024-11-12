using System.Reflection;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Client.Services;
using ButtsBlazor.Client.Utils;
using ButtsBlazor.Client.ViewModels;
using ButtsBlazor.Services;
using ButtsBlazor.Shared.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using PubSub;

namespace ButtsBlazor.Api.Services;

public static class PromptConfig
{
    public static IHostApplicationBuilder AddButts(this IHostApplicationBuilder @this)
    {
        @this.Services.AddSingleton<NotificationService>();
        @this.Services.AddSingleton<ButtsListFileService>();
        @this.Services.AddSingleton<FileService>();
        @this.Services.AddSingleton<ImagePathService>();
        @this.Services.Configure<PromptOptions>(@this.Configuration.GetSection(nameof(PromptOptions)));
        @this.Services.AddSingleton(sp => sp.GetService<IOptions<PromptOptions>>()?.Value ?? throw new InvalidOperationException($"Could not find PromptOptions"));
        @this.Services.AddSingleton<PromptQueue>();

        @this.Services.AddTransient<IButtsNotificationClient, ServerNotificationClient>();
        @this.Services.AddScoped<Random>();
        @this.Services.AddScoped<IPromptGenerationService, SnickPromptGenerationService>();
        @this.Services.AddScoped<IButtsApiClient, FakeButtsApiClient>();

        return @this;
    }

}


public class FakeButtsApiClient : IButtsApiClient
{
    public Task<UploadResult> UploadFile(string dataUrlString, string prompt, string inputImage, string code,
        ImageType? imageType = null)
    {
        return Task.FromResult<UploadResult>(new UploadResult());
    }

    public Task<UploadResult> UploadFile(IBrowserFile file)
    {
        return Task.FromResult<UploadResult>(new UploadResult());
    }
    public Task<WebPath[]> GetRecentImages(int numImages, ImageType? type = null)
    {
        return Task.FromResult(Array.Empty<WebPath>());
    }

    public Task<UploadResult> UploadFile(Stream stream, string contentType, string name, IEnumerable<KeyValuePair<string, string>>? additionalFormData = null)
    {
        return Task.FromResult<UploadResult>(new UploadResult());
    }

    public Task<UploadResult> UploadFile(HttpClient client, Stream stream, string contentType, string name,
        IEnumerable<KeyValuePair<string, string>>? additionalFormData = null)
    {
        return Task.FromResult<UploadResult>(new UploadResult());
    }
}