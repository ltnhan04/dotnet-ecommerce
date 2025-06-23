using System.Collections.Generic;
using System.Threading.Tasks;
using api.Dtos;
public interface ILocationService
{
	Task<List<ProvinceDto>> GetProvincesAsync();
	Task<DistrictDto> GetDistrictsByProvinceCodeAsync(int code);
	Task<WardDto> GetWardsByDistrictCodeAsync(int code);
}
