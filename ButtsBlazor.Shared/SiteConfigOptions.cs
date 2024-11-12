using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Web;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Client.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MQTTnet.Client;

namespace ButtsBlazor.Api.Services;

public static class SiteConfigOptionsExtensions
{

    public static IConfigurationRoot BuildDefaultConfiguration(this IConfigurationBuilder @this,
        string? basePath = null, Assembly? secretsAssembly = null)
    {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        return @this
            .AddJsonFile("appsettings.json", true, true)
            .SetBasePath(basePath ?? Path.GetDirectoryName(assembly.Location) ?? Directory.GetCurrentDirectory())
            .AddUserSecrets(
                secretsAssembly ?? Assembly.GetCallingAssembly() ??
                Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly(), true)
            .AddEnvironmentVariables()
            .Build();
    }
    
    public static SiteConfigOptions AddSiteConfig(this IHostApplicationBuilder @this) => AddSiteConfig(@this, new ConfigurationBuilder().BuildDefaultConfiguration());

    public static SiteConfigOptions AddSiteConfig(this IHostApplicationBuilder @this, IConfigurationRoot root)
    {
        return @this.AddConfig<SiteConfigOptions>(root,SiteConfigOptions.SectionName);
    }
    public static T AddConfig<T>(this IHostApplicationBuilder @this, string sectionName) where T : class, new() =>
        @this.AddConfig<T>(new ConfigurationBuilder().BuildDefaultConfiguration(), sectionName);

    public static T AddConfig<T>(this IHostApplicationBuilder @this, IConfigurationRoot root, string sectionName) where T : class, new()
    {
        var section = root.GetSection(sectionName);
        var config = section.Get<T>() ?? new();
        @this.Services.Configure<T>(section);
        @this.Services.AddSingleton(sp => sp.GetService<IOptions<T>>()?.Value ?? throw new InvalidOperationException($"Could not find SiteConfigOptions"));
        return config;
    }
}

public class SiteConfigOptions 
{
    public ClusterConfig Cluster { get; set; } = new();
    public const string SectionName = "SiteConfig";
    public string RootCssClass { get; set; } = "infinite";
    internal string HomeTitle { get; set; } = "Protobooth - Mystical Forest Photos!";
    public string DecodedHomeTitle => HomeTitle.Replace("*", "'");
    public string HomeDescription { get; set; } = "Stream photos from the photo-proto-booth!";
    public string BackgroundImage { get; set; } = "/bg.png";
    public string LoaderImage { get; set; } = "/infinitypeach.svg";
    public string FavIcon{ get; set; } = "/favicon.png";
    public string TabletIcon { get; set; } = "/favicon.png";
    public string? BackgroundBlur { get; set; }
    public string FontWeight {get;set;} = "normal";
    public string FontColor {get;set;}  = "white";
    public int RandomImageDisplaySeconds {get;set;} = 15;
    public int NewImageDisplaySeconds { get; set; } = 15;
    public string? GoogleFontFamily {get;set;}
    public bool IsWhiteTransparent { get; set; }
    public ImageType DefaultImageType { get; set; } = ImageType.Photo;
    public string BoxColor { get; set; } = "#DDDE";
    public string DbPath { get;  set; } = @"db/butts.db";
    public string FullDbPath => Path.GetFullPath(DbPath);
    public string FontSize { get; set; } = "5vh";
    public string? DefaultMetaImage { get; set; }
    public int? IndexRefreshSeconds { get; set; } = 45;
    public string Name { get; set; } = "espam";

    private static string? _csshash;
    private string AppStylesheetHash => _csshash ??= CalculateMD5(Path.Combine("wwwroot", AppStylesheet));
    public string AppStylesheet { get; set; } = "app.css";
    public string StylsheetHref => $"{AppStylesheet}?v={HttpUtility.UrlEncode(AppStylesheetHash)}";

    static string CalculateMD5(string filename)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filename))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
    public string MqttTopic(string subKey) =>
        $"{Name}/{MqttBaseTopic}/{subKey}".ToLowerInvariant();
    public string MqttTopic(ImageType? image) =>
        MqttTopic(image?.ToString() ?? "unknown");  
    public string MqttBaseTopic { get; set; } = "images";
}