using ButtsBlazor.Api.Model;
using ButtsBlazor.Api.Services;
using ButtsBlazor.Client.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace ButtsBlazor.Server.Pages
{
    public class ConfigModel : PageModel
    {
        private readonly ButtsDbContext db;
        private readonly IOptionsSnapshot<PromptOptions> configMonitor;

        public ConfigModel(IOptionsSnapshot<PromptOptions> configMonitor, ButtsDbContext db)
        {
            this.db = db;
            this.configMonitor = configMonitor;
        }

        [FromForm] public PromptOptions Input { get; set; } = null!;
        public void OnGet()
        {
            Input = configMonitor.Value;
        }

        //public async Task OnPost()
        //{
        //    if (Input.GradioUri != configMonitor.Value.GradioUri)
        //    {

        //    }
        //}
    }
}
