// See https://aka.ms/new-console-template for more information

using ButtsBlazor.Api.Services;
using ButtsBlazor.Printer;
using ButtsBlazor.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole();
var printConfig = builder.AddConfig<PrintConfig>(
    SiteConfigOptions.SectionName + ":" + PrintConfig.SectionName);
printConfig.SiteConfig = builder.AddSiteConfig();
//printConfig.SiteConfig.Cluster.ClientId = "ButtsBlazor.Printer";
builder.AddMqtt(printConfig.SiteConfig.Cluster)
    .Services.AddHostedService<ImageUploadMonitor>()
    .AddSingleton<PrintQueue>();

IHost host = builder.Build();
host.Run();
