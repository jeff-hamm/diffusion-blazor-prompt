using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ButtsBlazor.Server.Pages
{
    public class PhotoModel : PageModel
    {
        public ActionResult OnGet()
        {
            var b = String.IsNullOrEmpty(Request.PathBase) ? "/" : Request.PathBase.ToString(); 
	        return RedirectPermanent(b + "?t=photo");
        }
    }
}
