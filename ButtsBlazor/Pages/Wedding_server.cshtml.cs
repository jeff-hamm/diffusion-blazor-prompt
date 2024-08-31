using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ButtsBlazor.Server.Pages;

public class WeddingModel : PageModel
{
	[FromQuery]public string? Image { get; set; }
	[FromQuery]public string? Prompt { get; set; }
    [FromQuery] public string Tenant { get; set; } = "wedding";
	public void OnGet()
	{

	}
}