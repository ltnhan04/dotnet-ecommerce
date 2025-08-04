using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using api.Dtos;
using api.models;

namespace api.Pages.Admin.Customers
{
    [Authorize(Roles = "admin")]
    public class Details : PageModel
    {
        private readonly ILogger<Details> _logger;
        private readonly HttpClient _httpClient;

        public User? Customer { get; set; }
        public GetCustomerPointDto? CustomerPoints { get; set; }
        public List<GetCustomerVoucherDto> CustomerVouchers { get; set; } = new();

        public Details(ILogger<Details> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVER_URL")!);
        }

        public async Task OnGetAsync(string id)
        {
            var token = Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            try
            {
                var customerResponse = await _httpClient.GetAsync($"v1/admin/customers/{id}");
                if (customerResponse.IsSuccessStatusCode)
                {
                    var customerJson = await customerResponse.Content.ReadAsStringAsync();
                    var customerDoc = JsonDocument.Parse(customerJson);
                    Customer = JsonSerializer.Deserialize<User>(customerDoc.RootElement.GetProperty("data").ToString());
                }

                var pointsResponse = await _httpClient.GetAsync($"v1/admin/customers/{id}/points");
                if (pointsResponse.IsSuccessStatusCode)
                {
                    var pointsJson = await pointsResponse.Content.ReadAsStringAsync();
                    var pointsDoc = JsonDocument.Parse(pointsJson);
                    CustomerPoints = JsonSerializer.Deserialize<GetCustomerPointDto>(pointsDoc.RootElement.GetProperty("data").ToString());
                }

                var vouchersResponse = await _httpClient.GetAsync($"v1/admin/customers/{id}/vouchers");
                if (vouchersResponse.IsSuccessStatusCode)
                {
                    var vouchersJson = await vouchersResponse.Content.ReadAsStringAsync();
                    var vouchersDoc = JsonDocument.Parse(vouchersJson);
                    CustomerVouchers = JsonSerializer.Deserialize<List<GetCustomerVoucherDto>>(vouchersDoc.RootElement.GetProperty("data").ToString()) ?? new();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer details");
            }
        }
    }
}