using System.Management.Automation;
using IronPrint;
using Microsoft.Extensions.Logging;

namespace ButtsBlazor.Printer;

public class PrintQueue(PrintConfig config, ILogger<PrintQueue> logger)
{
    public async Task Print(PrintJob printJob, CancellationToken token)
    {
        try
        {
            var printer = await DoesPrinterExist(config.PrinterName);
            var document = await DownloadToBytes(printJob, token);
            logger.LogInformation("Printing {PrintJob} to {Printer}", printJob, printer);
            await IronPrint.Printer.PrintAsync(document,
                new PrintSettings()
                {
                    PrinterName = printer,
                });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error printing {PrintJob}", printJob);
        }
    }

    private async Task<string?> DoesPrinterExist(string printerName)
    {
        var printer =
            (await IronPrint.Printer.GetPrinterNamesAsync()).FirstOrDefault(p =>
                p.Equals(printerName, StringComparison.InvariantCultureIgnoreCase));

        if (printer == null)
        {
            logger.LogError("Printer {PrinterName} not found", printerName);
            return printer;
        }

        return printer;
    }

    private async Task<object> DownloadToFile(PrintJob printJob, CancellationToken token=default)
    {
        var fileName = Path.GetTempFileName();
        await using var fs = File.OpenWrite(fileName);
        await DownloadTo(printJob, fs,token);
        return fileName;
    }
    private async Task<byte[]> DownloadToBytes(PrintJob printJob, CancellationToken token=default)
    {
        using var ms = new MemoryStream();
        await DownloadTo(printJob, ms,token);
        ms.Position = 0;
        return ms.ToArray();
    }

    private async Task<Stream> DownloadTo(PrintJob printJob,Stream dst, CancellationToken token)
    {
        using var client = new HttpClient();
        client.BaseAddress = printJob.UploadResult.BaseUrl ?? config.SiteConfig.Cluster.PublicUri;
        logger.LogDebug("Downloading file from {uri}", printJob.UploadResult.FullUri);
        await using var stream = await client.GetStreamAsync(printJob.UploadResult.Path, token);
        await stream.CopyToAsync(dst, token);
        return dst;
    }
}