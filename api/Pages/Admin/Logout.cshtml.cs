using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace api.Pages.Admin
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public LogoutModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                Response.Cookies.Delete("accessToken");
                var refreshToken = Request.Cookies["refreshToken"];
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    await _httpClient.PostAsync($"/api/v1/auth/logout", null);
                }
                return RedirectToPage("/Admin/Index");
            }
            catch
            {
                return RedirectToPage("/Admin/Index");
            }
        }
    }
}