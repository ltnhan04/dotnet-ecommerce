using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using api.Dtos;
using api.Interfaces;
		

public class LocationService : ILocationService
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly string _baseUrl;
public LocationService(IHttpClientFactory httpClientFactory)
{
	_httpClientFactory = httpClientFactory;
	_baseUrl = Environment.GetEnvironmentVariable("BASE_URL")!;
		

	if (!_baseUrl.EndsWith("/"))
	{
	    _baseUrl += "/";
	}
}
		

public async Task<List<ProvinceDto>> GetProvincesAsync()
{
    var client = _httpClientFactory.CreateClient();
    var response = await client.GetAsync($"{_baseUrl}p/");
    response.EnsureSuccessStatusCode();
		

	var content = await response.Content.ReadAsStringAsync();
	return JsonSerializer.Deserialize<List<ProvinceDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
}
		

public async Task<List<DistrictDto>> GetDistrictsByProvinceCodeAsync(int code)
{
    var client = _httpClientFactory.CreateClient();
    var url = $"{_baseUrl}p/{code}?depth=2";
    var response = await client.GetAsync(url);
    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadAsStringAsync();
    // Log nội dung trả về để debug
    Console.WriteLine($"[DEBUG] API Response for {url}: {content}");

    var province = JsonSerializer.Deserialize<ProvinceDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    if (province == null)
    {
        Console.WriteLine("[ERROR] Deserialize ProvinceDto failed!");
        return new List<DistrictDto>();
    }
    if (province.districts == null)
    {
        Console.WriteLine("[ERROR] ProvinceDto.districts is null!");
        return new List<DistrictDto>();
    }
    return province.districts;
}
		

public async Task<List<WardDto>> GetWardsByDistrictCodeAsync(int code)
{
    var client = _httpClientFactory.CreateClient();
    var response = await client.GetAsync($"{_baseUrl}d/{code}?depth=2");
    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadAsStringAsync();
    var district = JsonSerializer.Deserialize<DistrictDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    return district?.wards ?? new List<WardDto>();
}
}
