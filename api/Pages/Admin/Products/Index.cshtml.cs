using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using api.Dtos;
using System.Collections.Generic;

namespace api.Pages.Admin.Products
{
    [Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        public List<ProductDto> Products { get; set; } = new();

        public void OnGet()
        {
        }
    }
} 