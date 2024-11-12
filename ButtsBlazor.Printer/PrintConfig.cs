using ButtsBlazor.Api.Services;
using Microsoft.Extensions.Configuration;

public class PrintConfig
{
    public const string SectionName =  nameof(PrintConfig);
    public SiteConfigOptions SiteConfig { get; set; } = new();
    public int PrintQueueLength { get; set; } = 5;
    public string PrinterName { get; set; } = "Canon SELPHY CP1500";
}