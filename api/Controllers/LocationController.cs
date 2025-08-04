using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using api.Interfaces;
		

[ApiController]
[Route("v1/")]
public class LocationController : ControllerBase
{
	private readonly ILocationService _locationService;
		

	public LocationController(ILocationService locationService)
	{
		_locationService = locationService;
	}
		

	[HttpGet("provinces")]
	public async Task<IActionResult> GetProvinces()
	{
		var provinces = await _locationService.GetProvincesAsync();
		return Ok(provinces);
	}
		

[HttpGet("districts/{code}")]
public async Task<IActionResult> GetDistrictsByProvinceCode(int code)
{
    var districts = await _locationService.GetDistrictsByProvinceCodeAsync(code);
    if (districts == null || districts.Count == 0)
    {
        return NotFound();
    }
    return Ok(districts);
}
		

	[HttpGet("wards/{code}")]
	public async Task<IActionResult> GetWardsByDistrictCode(int code)
	{
		var wards = await _locationService.GetWardsByDistrictCodeAsync(code);
		if (wards == null || wards.Count == 0)
		{
			return NotFound();
		}
		return Ok(wards);
	}
}
