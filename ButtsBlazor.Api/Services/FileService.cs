using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Api.Utils;
using ButtsBlazor.Client.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ButtsBlazor.Services
{
    public enum FileTypes
    {
        ControlImage,
        OutputImage,
    }
    public class FileService(IWebHostEnvironment env, PromptOptions config, IDbContextFactory<ButtsDbContext> dbContextFactory)
    {

        public async Task<string?> ReadAsBase64Contents(string? relativePath)
        {
            if (String.IsNullOrEmpty(relativePath))
                return null;
            var path = ToFilePath(relativePath);
            if (!File.Exists(path))
                return null;
            await using var fs = File.OpenRead(path);
            return await fs.ConvertToBase64Async();

        }

        public string? TryFindUploadFile(string controlImagePathOrHash)
        {
            var path = ToFilePath(controlImagePathOrHash);
            if (File.Exists(path))
                return controlImagePathOrHash;
            path = ToRelativeUploadPath(controlImagePathOrHash);
            if (File.Exists(path))
                return controlImagePathOrHash;
            if (TryFindExistingUpload(controlImagePathOrHash, out path))
                return path;
            return null;
        }

        public async ValueTask<SaveFileResult> SaveAndHashUploadedFile(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!config.SupportedImageExtensions.Contains(ext))
                throw new InvalidOperationException($"No support for file extension ext");
            var tempPath = ToFilePath(config.TempUploadsPath, Path.GetRandomFileName());
            await using var stream = file.OpenReadStream();
            try
            {
                var hash = await stream.SaveAndHashAsync(tempPath);
                var base64Hash = Convert.ToBase64String(hash);
                var relativePath = ToRelativeUploadPath(FileUtils.Base64ToFileName(base64Hash, ext));
                if (!RelativeFileExists(relativePath))
                {
                    var outputPath = ToFilePath(env.WebRootPath, relativePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ??
                                              throw new InvalidOperationException(
                                                  $"Could not get directory name for {outputPath}"));
                    File.Move(tempPath, outputPath);
                    await using var dbContext = await dbContextFactory.CreateDbContextAsync();
                    dbContext.Images.Add(new ImageEntity()
                    {
                        Id = Guid.NewGuid(),
                        Path = relativePath,
                        Base64Hash = base64Hash,
                        Type = ImageType.ControlInput
                    });
                    await dbContext.SaveChangesAsync();
                }
                return new SaveFileResult(relativePath, base64Hash);
            }
            finally
            {
                if(File.Exists(tempPath))
                    File.Delete(tempPath);
            }
        }

        public HashedImage? GetLatestUpload()
        {
            var di = new DirectoryInfo(ToFilePath(UploadPath)).EnumerateFiles().MaxBy(f => f.LastWriteTime);
            if (di == null) return null;
            var path = Path.GetRelativePath(env.WebRootPath, di.FullName);
            var hash = Path.GetFileNameWithoutExtension(path);
            return new HashedImage(path, hash);
        }

        public bool TryFindExistingUpload(string base64Hash, [NotNullWhen(true)] out string? relativePath)
        {
            relativePath = Directory.EnumerateFiles(ToFilePath(UploadPath), FileUtils.Base64ToFileNameWithoutExtension(base64Hash) + ".*").FirstOrDefault();
            return relativePath != null;
        }
        public bool UploadedFileExists(string base64Hash, string extension) =>
            File.Exists(ToFilePath(ToRelativeUploadPath(FileUtils.Base64ToFileName(base64Hash, extension))));
        public bool RelativeFileExists(string relativePath) =>
            File.Exists(ToFilePath(relativePath));

        public string UploadPath => Path.Combine(config.ImagePathRoot, config.ImageUploadsPath);
        public string OutputPath => Path.Combine(config.ImagePathRoot, config.ImageOutputPath);

        public string ToRelativeUploadPath(string fileName) =>
            Path.Combine(UploadPath, fileName);
        public string ToRelativePath(string pathBase, string fileName) =>
            Path.Combine(pathBase, fileName);


        public string ToFilePath(string relativePath) =>
            Path.Combine(env.WebRootPath, relativePath);
        public string ToFilePath(string relativeDirectory, string relativePath) =>
            ToFilePath(Path.Combine(relativeDirectory, relativePath));

        public async Task<HashedImage> SaveOutputFile(string prompt, string base64File, string extension)
        {
            var baseFilename = FileDatePrefix +
                               $"{FileUtils.ReplaceInvalidFileCharacters(prompt, "_")}";
            var fileName = baseFilename + extension;
            var ix = 0;
            while (File.Exists(fileName))
            {
                ix++;
                fileName = baseFilename + $"-({ix})" + extension;
            }

            using var ms = new MemoryStream(Convert.FromBase64String(base64File));
            return await SaveFile(ms, OutputPath,fileName);
        }

        private async Task<HashedImage> SaveFile(Stream ms, string pathBase, string fileName)
        {
            var relativePath = ToRelativePath(pathBase,fileName);
            var path = ToFilePath(relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException());
            var hash = await ms.SaveAndHashAsync(path);
            return new HashedImage(relativePath, Convert.ToBase64String(hash));
        }

        private static string FileDatePrefix => DateTime.UtcNow.ToString("yyyyMMddhhmmss") + "-";

        public async Task<HashedImage> SaveCannyFile(Guid id, IFormFile file)
        {
            var fileName = FileDatePrefix + "_" + id.ToString("N") + "_canny" + Path.GetExtension(file.FileName);
            await using var fs = file.OpenReadStream();
            var cannyFile = await SaveFile(fs, OutputPath, fileName);
            await using var dbContext = await dbContextFactory.CreateDbContextAsync();
            var prompt = await dbContext.Prompts.FindAsync(id);
            if (prompt != null)
            {
                prompt.CannyImage = new ImageEntity()
                {
                    Id = Guid.NewGuid(),
                    Path = cannyFile.Uri,
                    Base64Hash = cannyFile.Base64Hash,
                    Type = ImageType.Canny
                };
                await dbContext.SaveChangesAsync();
            }

            return cannyFile;
        }
    }

    public record SaveFileResult(string RelativePath, string Base64Hash);
}
