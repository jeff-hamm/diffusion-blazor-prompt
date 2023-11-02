using System.Data;
using System.Diagnostics.CodeAnalysis;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Api.Utils;
using ButtsBlazor.Client.Services;
using ButtsBlazor.Client.Utils;
using ButtsBlazor.Services;
using ButtsBlazor.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ButtsBlazor.Api.Services;

public class FileService(ImagePathService pathService, PromptOptions config, IDbContextFactory<ButtsDbContext> dbContextFactory, ILogger<FileService> logger)
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
    public async ValueTask<ImageEntity> SaveAndHashUploadedFile(string fileName, ImageType imageType, Func<Stream> readStream)
    {
        if (!pathService.TryGetValidExtension(fileName, out var extension))
            throw new InvalidOperationException($"No support for file extension ext");
        var tempPath = pathService.GetTempFilePath();
        await using var stream = readStream();
        try
        {
            var hash = await stream.SaveAndHashAsync(tempPath);
            var base64Hash = Convert.ToBase64String(hash);
            var relativePath = pathService.Image(imageType,FileUtils.Base64ToFileName(base64Hash, extension));
            var image = new ImageEntity()
            {
                Path = relativePath,
                Base64Hash = base64Hash,
                Type = imageType
            };
            if (relativePath.FilePath is {Exists:false} outputPath)
            {
                outputPath.EnsureDirectory();
                tempPath.Move(outputPath);
                await SaveDbImage(image);
            }
            return image;
        }
        finally
        {
            tempPath.Delete();
        }
    }

    private async Task SaveDbImage(ImageEntity image)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Images.Add(image);
        await dbContext.SaveChangesAsync();
    }

    private readonly ReaderWriterLock recentLock = new ReaderWriterLock();
    private const int LockTimeout = 1000;
    private WebPath[]? recentList;
    private int imageCount;

    public async Task<WebPath[]> GetLatestUploads(int count)
    {

        try
        {
            await using var db = await dbContextFactory.CreateDbContextAsync();
            recentLock.AcquireReaderLock(LockTimeout);
            await RefreshRecent(db);
            return random.RemoveNext(recentList.ToList(),count).ToArray();
        }
        finally
        {
            recentLock.ReleaseReaderLock();
        }
        //var di = new DirectoryInfo(ToFilePath(UploadPath)).EnumerateFiles().MaxBy(f => f.LastWriteTime);
        //if (di == null) return null;
        //var path = Path.GetRelativePath(env.WebRootPath, di.FullName);
        //var hash = Path.GetFileNameWithoutExtension(path);
        //return new HashedImage(path, hash);
    }

    [MemberNotNull("recentList")]
    internal async Task RefreshRecent(ButtsDbContext db)
    {
        int? newCount = null;
        if (recentList == null || (newCount = (await db.Images.CountAsync())) != imageCount)
        {
            LockCookie? lockCookie = null;
            if (recentLock.IsReaderLockHeld)
                lockCookie = recentLock.UpgradeToWriterLock(LockTimeout);
            else
                recentLock.AcquireWriterLock(LockTimeout);
            try
            {
                imageCount = newCount ?? await db.Images.CountAsync();
                recentList = 
                    await db.Images.Where(i => i.Type != ImageType.Output).OrderByDescending(i => i.RowId)
                    .Select(s => s.Path).Take(50).ToArrayAsync();
            }
            finally
            {
                if (lockCookie.HasValue)
                {
                    var v = lockCookie.Value;
                    recentLock.DowngradeFromWriterLock(ref v);
                }
                else
                    recentLock.ReleaseWriterLock();
            }
        }
    }


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
        await dbContext.SaveChangesAsync();
        return img;
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
            ImageEntity = saveFileResult
        };
        dbContext.Add(metadata);
        await dbContext.SaveChangesAsync();
        return metadata;
    }
}