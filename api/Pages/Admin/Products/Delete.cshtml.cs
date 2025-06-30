using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using api.Dtos;

namespace api.Pages.Admin.Products
{
    [Authorize(Roles = "admin")]
    public class DeleteModel : PageModel
    {
        public ProductDto Product { get; set; } = new();

        public void OnGet(string id)
        {
        }
    }
}