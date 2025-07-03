using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace api.Pages.Admin.Orders
{   
    [Authorize(Roles = "admin")]
    public class IndexModel : PageModel
    {
        public readonly HttpClient _httpClient;
        public List<AdminGetAllOrder> Orders { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalCount { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public new int Page { get; set; } = 1;
        
        [BindProperty(SupportsGet = true)]
        public int Size { get; set; } = 10;

        public IndexModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }

        public async Task OnGetAsync()
        {
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            
            var response = await _httpClient.GetAsync($"api/v1/admin/orders?page={Page}&size={Size}");
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonDocument.Parse(json).RootElement.GetProperty("data");
            CurrentPage = data.GetProperty("currentPage").GetInt32();
            TotalCount = data.GetProperty("total").GetInt32();
            TotalPages = (int)Math.Ceiling((double)TotalCount / Size); 
            Orders = JsonSerializer.Deserialize<List<AdminGetAllOrder>>(data.GetProperty("items").ToString())!;
        }
    }
}