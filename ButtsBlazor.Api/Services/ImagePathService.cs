using System.Diagnostics.CodeAnalysis;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Client.Utils;
using Microsoft.AspNetCore.Hosting;

namespace ButtsBlazor.Services;

public class ImagePathService : IPathConverter
{
    private readonly IWebHostEnvironment env;
    private readonly PromptOptions config;

    public ImagePathService(IWebHostEnvironment env, PromptOptions config)
    {
        this.env = env;
        this.config = config;
        PathConverterAccessor.SetPathConverter(this);
    }
    public FilePath ToFilePath(WebPath webPath) =>
        new (Path.Combine(env.WebRootPath, webPath.Path));

    public WebPath ToWebPath(FilePath path) =>
        new (Path.GetRelativePath(env.WebRootPath, path).Replace('\\', '/'));

    public WebPath WebPath(string directory) =>
        new(Path.Combine(config.ImagePathRoot, directory));
    public WebPath WebPath(string directory, string path) =>
        new (Path.Combine(config.ImagePathRoot,directory, path));

    public WebPath Image(ImageType type, string fileName) =>
        WebPath(ImageDirectoryLookup(type), fileName);
    public WebPath Directory(ImageType type) =>
        WebPath(ImageDirectoryLookup(type));
    public WebPath ThumbnailPath(WebPath path)
    {

        var filePath = Path.GetRelativePath(config.ImagePathRoot, path);
        return WebPath(ImageDirectoryLookup(ImageType.Thumbnail), filePath);
    }

    private string ImageDirectoryLookup(ImageType type) =>
        type switch
        {
            ImageType.Temp => config.TempUploadsPath,
            ImageType.Camera => config.CameraPath,
            ImageType.Upload => config.UserUploadsPath,
            ImageType.ControlNet => config.ControlNetPath,
            ImageType.Output => config.OutputPath,
            ImageType.Infinite => config.GeneratedPath,
            ImageType.Thumbnail => config.ThumbnailPath,
            _ => throw new InvalidOperationException($"Unknown image type {type}")
        };

    public bool TryGetValidExtension(string fileName, [NotNullWhen(true)] out string? extension)
    {
        var ext = Path.GetExtension(fileName).ToLower();
        extension = ext;
        return config.SupportedImageExtensions.Contains(ext);
    }

    public FilePath GetTempFilePath() => Image(ImageType.Temp, Path.GetRandomFileName()).FilePath;

}