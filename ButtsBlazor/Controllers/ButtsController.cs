﻿using ButtsBlazor.Api.Model;
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
using System.IO;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ButtsBlazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ButtsController(ButtsListFileService legacyFileService, FileService fileService,
        ILogger<ButtsController> logger, IOptionsSnapshot<PromptOptions> options,
        IOptionsSnapshot<SiteConfigOptions> siteOptions,
        IHubContext<NotifyHub> hub) : ControllerBase
    {
	    [HttpGet("photo")]
	    public Task<ButtImage>? GetPhotobooth([FromQuery] DateTime? known, [FromQuery] int? except) =>
		    Get(known, except, ImageType.Photo);
        [HttpGet("")]
        public async Task<ButtImage>? Get([FromQuery] DateTime? known, [FromQuery] int? except, [FromQuery]ImageType? type)
        {
            type ??= siteOptions.Value.DefaultImageType;
            //            known ??= DateTime.Now;
            bool isMostRecent = true;


            // TODO: fix it so that we show the recent ones in order 
            await foreach (var path in fileService.GetLatest(2, ImageType.Output))
            {
                if (isMostRecent)
                {
                    path.IsLatest = true;
                    isMostRecent = false;
                }
                else
                {
                    path.IsLatest = false;
                }
                if (path.Index != except && (!known.HasValue || path.Created > known))
                    return path;
            }
            isMostRecent = true;
            await foreach (var path in fileService.GetLatest(2, type ?? siteOptions.Value.DefaultImageType))
            {
                if (isMostRecent)
                {
                    path.IsLatest = true;
                    isMostRecent = false;
                }
                else
                {
                    path.IsLatest = false;
                }
                if (path.Index != except && (!known.HasValue || path.Created > known))
                    return path;
            }

            await foreach (var butt in fileService.GetRandom(1, type ?? siteOptions.Value.DefaultImageType))
            {

                butt.IsLatest = false;
                return butt;
            }

            return ButtImage.Empty;
        }

        // GET: api/<ValuesController>
        [HttpGet("latest")]
        public ButtImage? GetLatest([FromQuery]DateTime? known) =>
            legacyFileService.GetLatest(known);
        [HttpGet("random/{except:int?}")]
        public ButtImage? GetRandom(int? except) =>
            legacyFileService.GetRandom(except);

        // GET: api/<ValuesController>
        [HttpGet("{index:int}")]
        public async Task<ButtImage?> GetIndex(int index) =>
            legacyFileService.Index(index) ?? 
            ((await fileService.Get(index)) is {} r ?
            ToButt(r) : null);

        private ButtImage? ToButt(ImageEntity get) =>
	        new (get.Path, get.CreationDate, get.RowId, null);

        [HttpGet("fileScan")]
        public Task FileScan([FromQuery]ImageType? imageType = null) => fileService.FileScan(imageType);

        [HttpGet("options")]
        public PromptOptions GetOptions() => options.Value;

        [HttpGet("recent")]
        public async Task<IEnumerable<WebPath>> Get(int count, ImageType type = ImageType.Output)
        {
            return (await fileService.GetLatestAndRandom(count > 12 ? count - 4 : count-2, count, type)).Where(s => s.ThumbnailPath.HasValue).Select(s =>  s.ThumbnailPath!.Value);
        }

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

        private static readonly Regex ValidTenant = new(@"^[a-zA-Z0-9\-_]+$", RegexOptions.Compiled);
        [HttpPost("upload")]
        public async Task<ActionResult<UploadResult>> PostFile([FromForm] IFormFile file,
            [FromForm] string? prompt, [FromForm] string? code, [FromForm] string? inputImage, 
            [FromForm]ImageType? imageType, string tenant="butts"
        )
        {
            if(!ValidTenant.IsMatch(tenant))
				return BadRequest("Invalid tenant name");
            var uploadResult = new UploadResult<ImageEntity>();
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
                    uploadResult = await fileService.SaveUploadedFile(tenant, file.FileName, 
                        imageType ?? ImageType.Output, file.OpenReadStream);
                    uploadResult.BaseUrl = new Uri(siteOptions.Value.Cluster.PublicUrl);
                    logger.LogInformation("{FileName} saved at {Path} with {Hash}",
                        displayFileName, uploadResult.Path, uploadResult.Hash);
                    if(uploadResult.Data != null)
                        await hub.Clients.NewControlImage(uploadResult.Data);
                }
                catch (IOException ex)
                {
                    logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                        displayFileName, ex.Message);
                    uploadResult.Error = $"{displayFileName} error on upload (Err: 3): {ex.Message}";
                }
            }
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Allow-Control-Allow-Headers",
	            "origin, authorization, accept, content-type, x-requested-with, Location");
            Response.Headers.Add("Allow-Control-Allow-Methods", "GET, HEAD, POST, PUT, DELETE, TRACE, OPTIONS");
            return new CreatedResult(new Uri($"{Request.Scheme}://{Request.Host}/" + uploadResult.Path), uploadResult);
        }
    }
}
