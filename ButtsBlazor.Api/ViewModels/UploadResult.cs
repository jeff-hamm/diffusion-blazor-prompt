namespace ButtsBlazor.Client.ViewModels;

public class UploadResult
{
    public string? Hash { get; set; }
    public string? Path { get; set; }
    public string FileName { get; set; }
    public string Error { get; set; }
    public bool Uploaded { get; set; }
}