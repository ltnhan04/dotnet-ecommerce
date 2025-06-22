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
		

public async Task<DistrictDto> GetDistrictsByProvinceCodeAsync(int code)
{
	var client = _httpClientFactory.CreateClient();
	var response = await client.GetAsync($"{_baseUrl}p/{code}?depth=2");
	response.EnsureSuccessStatusCode();
		

	var content = await response.Content.ReadAsStringAsync();
	return JsonSerializer.Deserialize<DistrictDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
}
		

public async Task<WardDto> GetWardsByDistrictCodeAsync(int code)
{
	var client = _httpClientFactory.CreateClient();
	var response = await client.GetAsync($"{_baseUrl}d/{code}?depth=2");
	response.EnsureSuccessStatusCode();
		

	var content = await response.Content.ReadAsStringAsync();
	return JsonSerializer.Deserialize<WardDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
}
}
