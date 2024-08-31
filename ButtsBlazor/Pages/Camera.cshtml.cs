using System.Net;
using ButtsBlazor.Api.Model;
using ButtsBlazor.Api.Services;
using ButtsBlazor.Client.Utils;
using ButtsBlazor.Client.ViewModels;
using ButtsBlazor.Hubs;
using ButtsBlazor.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace ButtsBlazor.Server.Pages
{
    public class CameraModel : PageModel
    {
        private readonly ILogger<CameraModel> logger;
        private readonly PromptOptions options;
        private readonly FileService fileService;
        private readonly IHubContext<NotifyHub> hub;
        public UploadResult? uploadResult;

        public CameraModel(ILogger<CameraModel> logger, PromptOptions options, FileService fileService,
            IHubContext<NotifyHub> hub)
        {
            this.logger = logger;
            this.options = options;
            this.fileService = fileService;
            this.hub = hub;
        }

        public ActionResult OnGet()
        {
            return Page();
        }
        public async Task<ActionResult> OnPost([FromForm] IFormFile? file, string tenant="butts")
        {
            if (file == null) return Page();
            var it = ImageType.Camera;
            uploadResult = new UploadResult();
            var untrustedFileName = file.FileName;
            var displayFileName =
                WebUtility.HtmlEncode(untrustedFileName);

            if (file.Length == 0)
            {
                logger.LogInformation("{FileName} length is 0 (Err: 1)",
                    displayFileName);
                uploadResult.Error = displayFileName + " length is 0 ";
            }
            else if (file.Length > options.MaxFileSize)
            {
                logger.LogInformation("{FileName} of {Length} bytes is " +
                                      "larger than the limit of {Limit} bytes",
                    displayFileName, file.Length, options.MaxFileSize);
                uploadResult.Error =
                    $"{displayFileName} of {file.Length} bytes is larger than the limit of {options.MaxFileSize} bytes";
            }
            else
            {
                try
                {
                    var saveFileResult = await fileService.SaveAndHashUploadedFile(tenant,file.FileName, it, file.OpenReadStream);
                    uploadResult.Uploaded = true;
                    logger.LogInformation("{FileName} saved at {Path} with {Hash}",
                        displayFileName, saveFileResult.Path, uploadResult.Hash);
                    await hub.Clients.NewCameraImage(saveFileResult);
                    return new JsonResult(new
                    {
                        location = "/camera"

//                        location = "/prompt?image=" + saveFileResult.Path + "&source=camera"
                    });
                }
                catch (IOException ex)
                {
                       logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                        displayFileName, ex.Message);
                    uploadResult.Error = $"{displayFileName} error on upload (Err: 3): {ex.Message}";
                }
            }

            return Page();
        }
    }
}