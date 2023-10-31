using Microsoft.Extensions.Logging;

namespace ButtsBlazor.Client.Utils;


public class PromptOptions
{
    public string GradioUri { get; set; } = "https://butts.infinitebutts.com";
    public int MaxFileSize { get; set; } = 1024 * 1024 * 40;
    public int HistoryLength { get; set; } = 100;
    public string ControlNetPath { get; set; } = "control";
    public string TempUploadsPath { get; set; } = "temp_uploads";
    public string[] SupportedImageExtensions { get; set; } = new[] { ".png", ".jpg", ".tiff", ".jpeg" };
    public string ImagePathRoot { get; set; } = "butts_images";
    public string UserUploadsPath { get; set; } = "user_uploads";
    public string OutputPath { get; set; } = "prompted";
    public string GeneratedPath { get; set; } = "generated";
    public string CameraPath { get; set; } = "camera";

    public GenerationOptions GenerationOptions { get; set; } = new();
    public LogLevel LogLevel { get; set; } = LogLevel.Debug;
    public string NotifyHubPath { get; set; } = "/notify";
    public int ActivityTimeout { get; set; } = 60 * 1000;
    public int NumChoices { get; set; } = 4;
    public int NumItemsPerChoice { get; set; } = 6;
    public int NumGeneratedImages { get; set; } = 1;
}