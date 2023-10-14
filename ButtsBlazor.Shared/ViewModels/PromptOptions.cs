namespace ButtsBlazor.Client.Utils;

public class PromptOptions
{
    public int MaxFileSize { get; set; } = 1024 * 1024 * 40;
    public int HistoryLength { get; set; } = 100;
    public string ControlImagePath { get; set; } = "control";
    public string TempUploadsPath { get; set; } = "temp_uploads";
    public string[] SupportedImageExtensions { get; set; } = new[] { ".png", ".jpg", ".tiff", ".jpeg" };
    public string ImagePathRoot { get; set; } = "butts_images";
    public string ImageUploadsPath { get; set; } = "uploaded";
    public string ImageOutputPath { get; set; } = "output";
}