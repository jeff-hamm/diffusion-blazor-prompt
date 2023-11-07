using ButtsBlazor.Api.Model;
using ButtsBlazor.Api.Services;
using ButtsBlazor.Client.ViewModels;
using ButtsBlazor.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using ButtsBlazor.Client.Utils;
using ButtsBlazor.Hubs;
using Microsoft.AspNetCore.SignalR;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ButtsBlazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ButtsController(ButtsListFileService legacyFileService, FileService fileService,
        ILogger<ButtsController> logger, IOptionsSnapshot<PromptOptions> options, IHubContext<NotifyHub> hub) : ControllerBase
    {

        [HttpGet("")]
        public ButtImage? Get([FromQuery] DateTime? known, [FromQuery] int? except) => 
            legacyFileService.GetLatestOrRandom(known, except);

        // GET: api/<ValuesController>
        [HttpGet("latest")]
        public ButtImage? GetLatest([FromQuery]DateTime? known) =>
            legacyFileService.GetLatest(known);
        [HttpGet("random/{except:int?}")]
        public ButtImage? GetRandom(int? except) =>
            legacyFileService.GetRandom(except);

        // GET: api/<ValuesController>
        [HttpGet("{index:int}")]
        public ButtImage? GetIndex(int index) =>
            legacyFileService.Index(index);

        [HttpGet("fileScan")]
        public Task FileScan([FromQuery]ImageType? imageType = null) => fileService.FileScan(imageType);

        [HttpGet("options")]
        public PromptOptions GetOptions() => options.Value;

        [HttpGet("recent")]
        public IAsyncEnumerable<WebPath> Get(int count) =>
            fileService.GetLatestUploads(count);

        [HttpGet("exists")]
        public string? Get(string imageHash)
        {
            if (fileService.TryFindExistingUpload(imageHash, out var path))
                return path;
            return null;
        }

        [HttpPost("canny")]
        public async Task PostCanny([FromForm] Guid id, [FromForm] IFormFile file)
        {
            await fileService.SaveFile(id, file.FileName,file.OpenReadStream, ImageType.ControlNet);
            return;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<UploadResult>> PostFile([FromForm] IFormFile file,
            [FromForm] string? prompt, [FromForm] string? code, [FromForm] string? inputImage, [FromForm]ImageType? imageType
        )
        {
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
                    var saveFileResult = await fileService.SaveAndHashUploadedFile(file.FileName, 
                        imageType ?? ImageType.Output, file.OpenReadStream);
                    if (prompt != null || code != null)
                        await fileService.AttachImageMetadata(saveFileResult, prompt, code, inputImage);
                    uploadResult.Hash = saveFileResult.Base64Hash;
                    uploadResult.Path = saveFileResult.Path;
                    uploadResult.Uploaded = true;
                    logger.LogInformation("{FileName} saved at {Path} with {Hash}",
                        displayFileName, saveFileResult.Path, uploadResult.Hash);
                    await hub.Clients.NewControlImage(saveFileResult);
                }
                catch (IOException ex)
                {
                    logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                        displayFileName, ex.Message);
                    uploadResult.Error = $"{displayFileName} error on upload (Err: 3): {ex.Message}";
                }
            }
            return new CreatedResult(new Uri($"{Request.Scheme}://{Request.Host}/" + uploadResult.Path), uploadResult);
        }
    }
}
