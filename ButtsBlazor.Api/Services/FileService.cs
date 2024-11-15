﻿using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Api.Utils;
using ButtsBlazor.Client.Services;
using ButtsBlazor.Client.Utils;
using ButtsBlazor.Client.ViewModels;
using ButtsBlazor.Server.Services;
using ButtsBlazor.Services;
using ButtsBlazor.Shared.Services;
using ButtsBlazor.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using static System.Net.Mime.MediaTypeNames;
using Image = SixLabors.ImageSharp.Image;

namespace ButtsBlazor.Api.Services;

public class NotificationService(
    SiteConfigOptions siteConfig,
    MqttManagedConnectedClient clientFactory,
    ILogger<NotificationService> logger)
{

    public async Task NotifyListeners(UploadResult<ImageEntity> image)
    {
        if (image.Error != null) return;
        var payload = JsonSerializer.Serialize(image);
        logger.LogInformation("Publishing {payload} to {topic}", payload, siteConfig.MqttTopic(image.ImageType));
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(siteConfig.MqttTopic(image.ImageType))
            .WithRetainFlag()
//            .WithUserProperty(nameof(UploadResult.ImageType), image.ImageType?.ToString()??"")
            .WithPayload(payload)
            .Build();
        var client = await clientFactory();
        await client.EnqueueAsync(applicationMessage);

        //logger.LogInformation("Publishing {payload} to {topic}", payload, siteConfig.MqttBaseTopic);
        //applicationMessage = new MqttApplicationMessageBuilder()
        //    .WithTopic(siteConfig.MqttBaseTopic)
        //    .WithPayload(payload)
        //    .Build();
//        await client
//            .PublishAsync(applicationMessage, CancellationToken.None);

    }

}
public class FileService(ImagePathService pathService, 
    PromptOptions config, 
    SiteConfigOptions siteConfig, 
    NotificationService notificationService,
    IDbContextFactory<ButtsDbContext> dbContextFactory, 
    ILogger<FileService> logger)
{
    private readonly Random random = new Random();
    public async Task FileScan(ImageType? imageType = null)
    {
        ImageType[] imageTypes;
        if (imageType != null)
            imageTypes = new[] { imageType.Value };
        else
            imageTypes = Enum.GetValues<ImageType>();
        foreach (var it in imageTypes)
        {
            var root = pathService.Directory(it).FilePath;
            if (!Directory.Exists(root))
            {
                logger.LogInformation("Could not find path for {imageType} at {path}, skipping", it, root.Path);
                continue;
            }
            logger.LogInformation("Starting file scan for {imageType} in {path}", it,root.Path);
            await using var db = await dbContextFactory.CreateDbContextAsync();
            foreach (var file in root.EnumerateFiles("*")
                         .Where(f => config.SupportedImageExtensions.Contains(Path.GetExtension(f))))
            {
                var filePath = new FilePath(file);
                var webPath = filePath.WebPath;
                if (!db.Images.Any(i => i.Path == webPath) &&
                    await filePath.TryReadAsBase64Contents() is { } base64)
                    await AddImage(db, it, webPath, base64);
            }
        }
    }

    public async ValueTask<UploadResult<ImageEntity>> SaveUploadedFile(string tenant, string fileName, ImageType? imageType,
        Func<Stream> readStream)
    {
        var saveFileResult = await SaveAndHashUploadedFile(tenant, fileName, 
            imageType ?? ImageType.Output, readStream);
        var result = new UploadResult<ImageEntity>(saveFileResult)
        {
            ImageType = imageType,
            Hash = saveFileResult.Base64Hash,
            Path = saveFileResult.Path,
            Uploaded = true
        };
        await notificationService.NotifyListeners(result);
        return result;
    }

    public async ValueTask<ImageEntity> SaveAndHashUploadedFile(string tenant, string fileName, ImageType imageType, Func<Stream> readStream)
    {
        if (!pathService.TryGetValidExtension(fileName, out var extension))
            throw new InvalidOperationException($"No support for file extension ext");
        var tempPath = pathService.GetTempFilePath();
        await using var stream = readStream();
        try
        {
            var hash = await stream.SaveAndHashAsync(tempPath);
            var base64Hash = Convert.ToBase64String(hash);
            var relativePath = pathService.Image(imageType,FileUtils.Base64ToFileName(base64Hash, extension),tenant);
            var image = new ImageEntity()
            {
                Path = relativePath,
                Base64Hash = base64Hash,
                Type = imageType
            };
            if (relativePath.FilePath is not { Exists: false } outputPath) return image;
            outputPath.EnsureDirectory();
            tempPath.Move(outputPath);
            await SaveDbImage(image);
            return image;
        }
        finally
        {
            tempPath.Delete();
        }
    }


    private readonly SemaphoreSlim dbLock = new(1);
    private async Task SaveDbImage(ImageEntity image)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Images.Add(image);
        await SaveChanges(dbContext);
        try
        {
            await EnsureThumbnail(image.Path);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,$"Error saving thumbnail {image.Path} {ex}");
        }
    }

    private async Task<WebPath> EnsureThumbnail(WebPath path)
    {
        var thumbWebPath = pathService.ThumbnailPath(path);
        if (thumbWebPath.FilePath is { Exists: false } thumbPath)
        {
            thumbPath.EnsureDirectory();
            logger.LogInformation($"Resizing {path} to {thumbPath}");
            using var image = Image.Load<Rgb24>(path.FilePath);
            int height = 400;
            int width = 400;
            if (image.Width > image.Height)
                height = 0;
            else
                width = 0;
            image.Mutate(x => x.Resize(width, height));
            await image.SaveAsync(thumbPath);
        }

        return thumbWebPath;
    }

    private readonly SemaphoreSlim recentLock = new (1);
    private const int LockTimeout = 1000;
    private Dictionary<ImageType, ButtImage[]>? recentList = null;

    public async Task<IEnumerable<ButtImage>> GetLatestAndRandom(int maxLatestCount, int totalCount, ImageType imageType)
    {
        var paths = new List<ButtImage>();
        await foreach (var recent in GetLatest(maxLatestCount, imageType))
        {
            if (paths.Count >= totalCount)
                return paths;
            paths.Add(recent);
        }

        var randomCount =
            (totalCount - paths.Count);
        await foreach (var recent in GetRandom(randomCount, siteConfig.DefaultImageType))
        {
            if (paths.Count >= totalCount)
                return paths;
            if (recent.ThumbnailPath == null)
                paths.Add( recent with
                {
                    ThumbnailPath = await EnsureThumbnail(recent.Path)
                });
            else
                paths.Add(recent);
        }
        //    paths.Count switch
        //{
        //    >= 12 => 4,
        //    >= 8 => 2,
        //    _ => 0
        //};
        random.Shuffle(paths);
        foreach (var path in paths)
        {

            await EnsureThumbnail(path.Path);
        }
        return paths;
    }

    public async IAsyncEnumerable<ButtImage> GetRandom(int count, ImageType? imageType)
    {
        imageType ??= siteConfig.DefaultImageType;
        await using var db = await dbContextFactory.CreateDbContextAsync();
        while (count > 0 && await NextRandom(db, imageType.Value) is { } randomImage)
        {
            if (!randomImage.Path.FilePath.Exists)
                continue;

            yield return randomImage;
            count--;
        }

    }

    public async IAsyncEnumerable<ButtImage> GetLatest(int count, ImageType type)
    {
            await using var db = await dbContextFactory.CreateDbContextAsync();
            var recentPaths = await RefreshRecent(db, type);

                foreach (var cameraImage in recentPaths)
                {
                    if(!cameraImage.Path.FilePath.Exists)
                        continue;
                    yield return cameraImage;
                    count--;
                    if (count <= 0)
                        break;

                }
        //var di = new DirectoryInfo(ToFilePath(UploadPath)).EnumerateFiles().MaxBy(f => f.LastWriteTime);
        //if (di == null) return null;
        //var path = Path.GetRelativePath(env.WebRootPath, di.FullName);
        //var hash = Path.GetFileNameWithoutExtension(path);
        //return new HashedImage(path, hash);
    }

    private readonly SemaphoreSlim randomImagesLock = new(1);
    private ConcurrentDictionary<ImageType,ConcurrentBag<ButtImage>> randomImagesByType = new();
    internal async Task<ButtImage?> NextRandom(ButtsDbContext db, ImageType? imageType)
    {
        imageType ??= siteConfig.DefaultImageType;
        var randomImages = randomImagesByType.GetOrAdd(imageType.Value, it => new ConcurrentBag<ButtImage>());
        if (randomImages.TryTake(out var path))
            return path;
        await randomImagesLock.WaitAsync();
        try
        {
            if (randomImages.TryTake(out path))
                return path;
            await foreach (var item in db.Images.Where(i => i.Type == imageType)
                               .OrderBy(i => EF.Functions.Random())
                               .Select(i => 
                                   new ButtImage(i.Path, i.CreationDate, i.RowId,null))
                               .Take(50).AsAsyncEnumerable())
                randomImages.Add(item);
            if(randomImages.TryTake(out path))
                return path with
                {
                    ThumbnailPath = pathService.ThumbnailPath(path.Path)
                };
            return null;
        }
        finally
        {
            randomImagesLock.Release();
        }
    }

    private bool hasChanges;
//    private int lastRowId;
    [MemberNotNull("recentList")]
    internal async Task<ButtImage[]> RefreshRecent(ButtsDbContext db, ImageType imageType)
    {
        if (recentList != null && !hasChanges)
            return recentList.TryGetValue(imageType, out var val) ? val : Array.Empty<ButtImage>();
        //if (!await recentLock.WaitAsync(TimeSpan.FromSeconds(60)))
        //    throw new InvalidOperationException("Could not get recentLock");
        try {
            var recentTime = DateTime.Now.Subtract(TimeSpan.FromMinutes(config.ImagesRecentForMinutes));
            hasChanges = false;
//            lastRowId = newRowId ?? await GetLastRowId(db);
            recentList = await db.Images.Where(i => i.CreationDate > recentTime)
                .GroupBy(i => i.Type)
                .ToDictionaryAsync(g => g.Key, g => 
                    g.OrderByDescending(i => i.RowId)
                        .AsEnumerable()
                        .Select(s =>
                        new ButtImage(s.Path, s.CreationDate,s.RowId, pathService.ThumbnailPath(s.Path))).ToArray());
            return recentList.TryGetValue(imageType, out var val) ? val : Array.Empty<ButtImage>();
        }
        finally
        {
//            recentLock.Release();
        }

    }

    //private async Task<int> GetLastRowId(ButtsDbContext db)
    //{
    //    return await db.Images.OrderByDescending(i => i.RowId).Select(i => i.RowId).FirstOrDefaultAsync();
    //}


    public bool TryFindExistingUpload(string base64Hash, [NotNullWhen(true)] out string? relativePath)
    {
        relativePath = pathService.Directory(ImageType.Upload).FilePath.EnumerateFiles(
            FileUtils.Base64ToFileNameWithoutExtension(base64Hash) + ".*").FirstOrDefault();
        return relativePath != null;
    }
    //public bool UploadedFileExists(string base64Hash, string extension) =>
    //    pathService.GetUploadPath(FileUtils.Base64ToFileName(base64Hash, extension)).FilePath.Exists();
    //public bool RelativeFileExists(string relativePath) =>
    //    File.Exists(ToFilePath(relativePath));

    public async Task<HashedImage> SaveOutputFile(string prompt, string base64File, string extension)
    {
        var baseFilename = FileDatePrefix +
                           $"{FileUtils.ReplaceInvalidFileCharacters(prompt, "_")}";
        var fileName = pathService.Image(ImageType.Output, baseFilename + extension).FilePath;
        var ix = 0;
        while (fileName.Exists)
        {
            ix++;
            fileName = pathService.Image(ImageType.Output, baseFilename + $"-({ix})" + extension).FilePath;
        }

        using var ms = new MemoryStream(Convert.FromBase64String(base64File));
        return await SaveFile(ms, fileName);
    }

    private async Task<HashedImage> SaveFile(Stream ms, FilePath path)
    {
        var hash = await ms.SaveAndHashAsync(path);
        return new HashedImage(path.WebPath, Convert.ToBase64String(hash));
    }

    private static string FileDatePrefix => DateTime.UtcNow.ToString("yyyyMMddhhmmss") + "-";

    public async Task<HashedImage> SaveFile(Guid id, string fileName,Func<Stream> readStream, ImageType imageType)
    {
        var path = pathService.Image(imageType, FileDatePrefix + "_" + id.ToString("N") + "_" + imageType.ToString().ToLower() + Path.GetExtension(fileName));
        await using var fs = readStream();
        var cannyFile = await SaveFile(fs, path.FilePath);
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var prompt = await dbContext.Prompts.FindAsync(id);
        if (prompt != null)
        {
            var img = await AddImage(dbContext,imageType, cannyFile.Uri, cannyFile.Base64Hash);
            prompt.CannyImageId = img.RowId;

        }

        return cannyFile;
    }

    private async Task<ImageEntity> AddImage(ButtsDbContext dbContext,ImageType imageType, WebPath path, string hash)
    {
        var img = new ImageEntity()
        {
            Path = path,
            Base64Hash = hash,
            Type = imageType
        };
        dbContext.Add(img);
        logger.LogDebug("Inserting image at {path} with type {imageType} and {id}",img.Path,img.Type, img.RowId);
        await SaveChanges(dbContext);
        return img;
    }

    private async Task SaveChanges(ButtsDbContext dbContext)
    {
        await dbLock.WaitAsync();
        try
        {

            hasChanges =  (await dbContext.SaveChangesAsync()) > 0;
        }
        finally
        {
            dbLock.Release();
        }
    }

    public async Task<ImageMetadata> AttachImageMetadata(ImageEntity saveFileResult, string? prompt, string? code, string? inputImage)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var metadata = new ImageMetadata()
        {
            Prompt = prompt,
            Code = code,
            InputImage = inputImage,
            ImageEntityId = saveFileResult.RowId,
//            ImageEntity = saveFileResult
        };
        dbContext.Add(metadata);
        await SaveChanges(dbContext);
        return metadata;
    }

    public async Task<ImageEntity?> Get(int i)
    {
	    await using var db = await dbContextFactory.CreateDbContextAsync();
	    return await db.Images.FindAsync(i);
    }
}