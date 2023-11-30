using ButtsBlazor.Api.Services;
using ButtsBlazor.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ButtsBlazor.Server.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;
        private readonly ButtsListFileService fileService;

        public IndexModel(ILogger<IndexModel> logger, ButtsListFileService fileService)
        {
            this.logger = logger;
            this.fileService = fileService;
        }

        public ActionResult OnGet(string? pageName)
        {
            if (Request.Headers.UserAgent.ToString().Contains("Mozilla/5.0 (IPad; CPU OS 10_3_3 like Mac OS X)"))
                return RedirectToPage("./camera");


            PageName = pageName?.ToLower() ?? "";
            ButtImage? image = null;
            if (Int32.TryParse(PageName, out var pageNumber))
                image = fileService.Index(pageNumber);
            else if (PageName == "latest")
                image = fileService.GetLatest(null);
            Image = image ?? fileService.GetLatestOrRandom();
            ViewData[nameof(ButtImage)] = Image;
            return Page(); 
        }
        public ButtImage Image { get; set; } = ButtImage.Empty;
        public string? PageName { get; set; }
    }
}