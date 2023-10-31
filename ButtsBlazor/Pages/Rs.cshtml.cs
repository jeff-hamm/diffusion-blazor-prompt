using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ButtsBlazor.Server.Pages
{
    public class RsModel : PageModel
    {
        public ActionResult OnGet()
        {   
            return RedirectToPage("./Index");
        }
    }
}
