using System.Diagnostics.CodeAnalysis;
using ButtsBlazor.Client.Utils;
using ButtsBlazor.Client.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using System.IO;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;
using System.Text;

namespace ButtsBlazor.Services
{
    public enum FileTypes
    {
        ControlImage,
        OutputImage,
    }
    public class FileService(IWebHostEnvironment env, IOptions<PromptOptions> options)
    {
        private PromptOptions Config => options.Value;

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
            if (!Config.SupportedImageExtensions.Contains(ext))
                throw new UnsupportedContentTypeException($"No support for file extension ext");
            var tempPath = ToFilePath(Config.TempUploadsPath, Path.GetRandomFileName());
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
                }
                return new SaveFileResult(relativePath, base64Hash);
            }
            finally
            {
                if(File.Exists(tempPath))
                    File.Delete(tempPath);
            }
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

        public string UploadPath => Path.Combine(Config.ImagePathRoot, Config.ImageUploadsPath);
        public string OutputPath => Path.Combine(Config.ImagePathRoot, Config.ImageOutputPath);

        public string ToRelativeUploadPath(string fileName) =>
            Path.Combine(UploadPath, fileName);
        public string ToRelativeOutputPath(string fileName) =>
            Path.Combine(OutputPath, fileName);


        public string ToFilePath(string relativePath) =>
            Path.Combine(env.WebRootPath, relativePath);
        public string ToFilePath(string relativeDirectory, string relativePath) =>
            ToFilePath(Path.Combine(relativeDirectory, relativePath));

        public async Task<string?> SaveOutputFile(string prompt, string base64File, string extension)
        {
            var baseFilename = DateTime.UtcNow.ToString("yyyyMMddhhmmss") +
                               $"-{FileUtils.ReplaceInvalidFileCharacters(prompt, "_")}";
            var fileName = baseFilename + extension;
            var ix = 0;
            while (File.Exists(fileName))
            {
                ix++;
                fileName = baseFilename + $"-({ix})" + extension;
            }

            var relativePath = ToRelativeOutputPath(fileName);
//            await using var cs = new CryptoStream(fs, new FromBase64Transform(), CryptoStreamMode.Read);
            var path = ToFilePath(relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException());
            await using var fs = File.OpenWrite(path);
            fs.Write(Convert.FromBase64String(base64File));
            return relativePath;
        }
    }

    public record SaveFileResult(string RelativePath, string Base64Hash);
}
