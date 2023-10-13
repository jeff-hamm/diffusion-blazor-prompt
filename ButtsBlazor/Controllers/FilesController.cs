using ButtsBlazor.Invokable;
using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using ButtsBlazor.Client.Utils;
using ButtsBlazor.Client.ViewModels;
using ButtsBlazor.Services;
using Microsoft.Extensions.Options;

namespace ButtsBlazor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController(FileService fileService, IOptions<PromptOptions> options, ILogger<FilesController> logger) : ControllerBase
    {

        [HttpGet]
        public string? Get(string imageHash)
        {
            if (fileService.TryFindExistingUpload(imageHash, out var path))
                return path;
            return null;
        }

        [HttpPost]
        public async Task<ActionResult<UploadResult>> PostFile([FromForm] IFormFile file)
        {
            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
            var uploadResult = new UploadResult();
            var untrustedFileName = file.FileName;
            var displayFileName =
                WebUtility.HtmlEncode(untrustedFileName);

            if (file.Length == 0)
            {
                logger.LogInformation("{FileName} length is 0 (Err: 1)",
                    displayFileName);
                uploadResult.Error = displayFileName + " length is 0 ";
            }
            else if (file.Length > options.Value.MaxFileSize)
            {
                logger.LogInformation("{FileName} of {Length} bytes is " +
                    "larger than the limit of {Limit} bytes",
                    displayFileName, file.Length, options.Value.MaxFileSize);
                uploadResult.Error =
                    $"{displayFileName} of {file.Length} bytes is larger than the limit of {options.Value.MaxFileSize} bytes";
            }
            else
            {
                try
                {
                    var saveFileResult = await fileService.SaveAndHashUploadedFile(file);
                    uploadResult.Hash = saveFileResult.Base64Hash;
                    uploadResult.Path = saveFileResult.RelativePath;
                    uploadResult.Uploaded = true;
                    logger.LogInformation("{FileName} saved at {Path} with {Hash}",
                        displayFileName, saveFileResult.RelativePath, uploadResult.Hash);
                }
                catch (IOException ex)
                {
                    logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                        displayFileName, ex.Message);
                    uploadResult.Error = $"{displayFileName} error on upload (Err: 3): {ex.Message}";
                }
            }
            return new CreatedResult(resourcePath, uploadResult);
        }
    }  
}
