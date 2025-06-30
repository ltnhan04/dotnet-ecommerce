using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace api.Pages.Admin.Dashboard
{
    [Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}