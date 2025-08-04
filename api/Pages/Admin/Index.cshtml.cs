using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace api.Pages.Admin
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        [TempData]
        public string? ErrorMessage { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("admin"))
            {
                return RedirectToPage("/admin/dashboard/index");
            }
            ErrorMessage = null;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var loginDto = new
                {
                    email = Email,
                    password = Password,
                    role = "admin"
                };

                var response = await _httpClient.PostAsJsonAsync("/v1/auth/login", loginDto);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent);
                    if (loginResponse?.data?.accessToken != null)
                    {
                        Response.Cookies.Append("accessToken", loginResponse.data.accessToken, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Lax,
                            Path = "/",
                            Expires = DateTime.UtcNow.AddDays(7)
                        });

                        return RedirectToPage("/admin/dashboard/index");
                    }
                }

                var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent);
                ErrorMessage = error?.message ?? "Login failed";
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ErrorMessage = "An error occurred during login. Please try again.";
                return Page();
            }
        }
    }

    public class LoginResponse
    {
        public LoginData? data { get; set; }
        public string message { get; set; } = string.Empty;
    }

    public class LoginData
    {
        public string accessToken { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
    }

    public class ErrorResponse
    {
        public string message { get; set; } = string.Empty;
    }
}