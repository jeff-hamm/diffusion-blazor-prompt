using ButtsBlazor.Api.Services;
using ButtsBlazor.Server.Services;
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

        public void OnGet(string? pageName)
        {
            PageName = pageName?.ToLower() ?? "";
            ButtImage? image = null;
            if (Int32.TryParse(PageName, out var pageNumber))
                image = fileService.Index(pageNumber);
            else if (PageName == "latest")
                image = fileService.GetLatest(null);
            Image = image ?? fileService.GetLatestOrRandom();
            ViewData[nameof(ButtImage)] = Image;

        }
        public ButtImage Image { get; set; } = ButtImage.Empty;
        public string? PageName { get; set; }
    }
}