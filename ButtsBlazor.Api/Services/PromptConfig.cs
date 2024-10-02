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
using PubSub;

namespace ButtsBlazor.Api.Services;

public static class PromptConfig
{
    public static IConfigurationRoot BuildDefaultConfiguration(this ConfigurationBuilder @this,
        string? basePath = null, Assembly? secretsAssembly = null)
    {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        return @this
            .SetBasePath(basePath ?? Path.GetDirectoryName(assembly.Location) ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddUserSecrets(
                secretsAssembly ?? Assembly.GetCallingAssembly() ??
                Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly(), true)
            .AddEnvironmentVariables()
            .Build();
    }

    public static SiteConfigOptions AddSiteConfig(this IHostApplicationBuilder @this)
    {
        var root = new ConfigurationBuilder().BuildDefaultConfiguration();
        //        AppConfig ??= cfg.GetMipsHealthOptions(AppEnvironment.Test);
        var section = root.GetSection(SiteConfigOptions.SectionName);
        var config = section.Get<SiteConfigOptions>() ?? new();
        @this.Services.Configure<SiteConfigOptions>(section);
        @this.Services.AddSingleton(sp => sp.GetService<IOptions<SiteConfigOptions>>()?.Value ?? throw new InvalidOperationException($"Could not find SiteConfigOptions"));
        return config;
    }
        
    public static IHostApplicationBuilder AddButts(this IHostApplicationBuilder @this)
    {
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

public class SiteConfigOptions
{
    public const string SectionName = "SiteConfig";
    public string RootCssClass { get; set; } = "infinite";
    public string HomeTitle { get; set; } = "Infinite Butts - AI Butt Generator";
    public string HomeDescription { get; set; } = "An infinite stream of AI generated butts.";
    public string BackgroundImage { get; set; } = "/bg.png";
    public string LoaderImage { get; set; } = "/infinitypeach.svg";
    public string FavIcon{ get; set; } = "/favicon.png";
    public string? BackgroundBlur { get; set; }
    public string FontWeight {get;set;} = "normal";
    public string FontColor {get;set;}  = "white";
    public int RandomImageDisplaySeconds {get;set;} = 15;
    public int NewImageDisplaySeconds {get;set;} = 30;
    public string? GoogleFontFamily {get;set;}
    public bool IsWhiteTransparent { get; set; }
    public ImageType DefaultImageType { get; set; } = ImageType.Infinite;
    public string BoxColor { get; set; } = "#DDDE";
    public string DbPath { get;  set; } = @"db/butts.db";
    public string FullDbPath => Path.GetFullPath(DbPath);
    public string FontSize { get; set; } = "5vh";
    public string? DefaultMetaImage { get; set; }
    public int? IndexRefreshSeconds { get; set; } = 45;
}

public class FakeButtsApiClient : IButtsApiClient
{
    public Task<UploadResult> UploadFile(string dataUrlString, string prompt, string inputImage, string code)
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
}