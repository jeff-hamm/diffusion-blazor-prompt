using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using ButtsBlazor.Api.Utils;
using ButtsBlazor.Client.Utils;
using ButtsBlazor.Client.ViewModels;
using ButtsBlazor.Hubs;
using ButtsBlazor.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.SignalR;

namespace ButtsBlazor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlImagesController(FileService fileService, IOptions<PromptOptions> options,
        IHubContext<PromptHub> hub,
        ILogger<ControlImagesController> logger) : ControllerBase
    {

        [HttpGet]
        public HashedImage? Get() => fileService.GetLatestUpload();

        [HttpGet]
        public string? Get(string imageHash)
        {
            if (fileService.TryFindExistingUpload(imageHash, out var path))
                return path;
            return null;
        }

        [HttpPost("Canny")]
        public async Task PostCanny([FromForm]Guid id, [FromForm] IFormFile file)
        {
            await fileService.SaveCannyFile(id, file);
            return;
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
                    await hub.Clients.NewControlImage(new HashedImage(uploadResult.Path, uploadResult.Hash));
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
