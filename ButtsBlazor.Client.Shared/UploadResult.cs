using System;
using System.Text.Json.Serialization;
using ButtsBlazor.Api.Model;

namespace ButtsBlazor.Client.ViewModels;

public class UploadResult
{
    public Uri? BaseUrl { get; set; }

    public Uri? FullUri =>
        string.IsNullOrWhiteSpace(Path)
            ? null
            : (BaseUrl != null ? new Uri(BaseUrl, Path) : new Uri(Path, UriKind.Relative));
    public string? Hash { get; set; }
    public ImageType? ImageType { get; set; }
    public string? Path { get; set; }
    public string? Error { get; set; }
    public bool Uploaded { get; set; }
}

public class UploadResult<T>(T? data) : UploadResult where T : class
{
    public UploadResult() : this(null)
    {
    }

    [JsonIgnore] public T? Data { get; set; } = data;
}
