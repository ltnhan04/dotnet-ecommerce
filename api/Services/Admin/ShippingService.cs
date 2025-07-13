using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces.Services;
using api.Dtos;

namespace api.Services.Admin
{
    public class ShippingService : IShippingService
    {
        private readonly HttpClient _client;
        private readonly ILogger<ShippingService> _logger;
        private List<int> _hcmDistrictIds = new();
        private const int HCM_PROVINCE_ID = 202;
        private async Task<(int? districtId, string? wardCode)> ResolveAddressAsync(string shippingAddress)
        {
            if (!_hcmDistrictIds.Any())
                await LoadMasterDataAsync();

            var provincesRes = await _client.GetAsync("/shiip/public-api/master-data/province");
            var provinces = await provincesRes.Content.ReadFromJsonAsync<GHNProvinceResponse>();

            var province = provinces.Data.FirstOrDefault(p =>
                shippingAddress.Contains(p.ProvinceName, StringComparison.OrdinalIgnoreCase));
            if (province == null) return (null, null);

            var districtRes = await _client.PostAsJsonAsync("/shiip/public-api/master-data/district", new { province_id = province.ProvinceID });
            var districts = await districtRes.Content.ReadFromJsonAsync<GHNDistrictResponse>();

            var district = districts.Data.FirstOrDefault(d =>
                shippingAddress.Contains(d.DistrictName, StringComparison.OrdinalIgnoreCase));
            if (district == null) return (null, null);

            var wardRes = await _client.PostAsJsonAsync("/shiip/public-api/master-data/ward", new { district_id = district.DistrictID });
            var wards = await wardRes.Content.ReadFromJsonAsync<GHNWardResponse>();

            var ward = wards.Data.FirstOrDefault(w =>
                shippingAddress.Contains(w.WardName, StringComparison.OrdinalIgnoreCase));

            return (district.DistrictID, ward?.WardCode);
        }



        public async Task LoadMasterDataAsync()
        {
            var provincesRes = await _client.GetAsync("/shiip/public-api/master-data/province");
            var provincesData = await provincesRes.Content.ReadFromJsonAsync<GHNProvinceResponse>();

            var districtsRes = await _client.PostAsJsonAsync("/shiip/public-api/master-data/district", new { });
            var districtsData = await districtsRes.Content.ReadFromJsonAsync<GHNDistrictResponse>();

            _hcmDistrictIds = districtsData.Data
                .Where(d => d.ProvinceID == HCM_PROVINCE_ID)
                .Select(d => d.DistrictID)
                .ToList();
        }


        public ShippingService(IHttpClientFactory clientFactory, ILogger<ShippingService> logger)
        {
            _logger = logger;
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://dev-online-gateway.ghn.vn");
            _client.DefaultRequestHeaders.Add("Token", "f4da4676-5d7a-11f0-9b81-222185cb68c8");
            _client.DefaultRequestHeaders.Add("ShopId", "197092");
        }

        public async Task<ShippingMethodResponseDto> CalculateShippingFeeAsync(ShippingDto request)
        {
            var (toDistrictId, toWardCode) = await ResolveAddressAsync(request.ShippingAddress);
            if (toDistrictId == null || string.IsNullOrEmpty(toWardCode))
            {
                _logger.LogError("Can not analyst address: " + request.ShippingAddress);
                return null;
            }

            var ghnRequest = new
            {
                to_district_id = toDistrictId,
                to_ward_code = toWardCode,
                service_type_id = 2,
                // height = request.Height,
                // length = request.Length,
                weight = request.Weight,
                // width = request.Width,
                // insurance_value = request.InsuranceValue
            };

            var response = await _client.PostAsJsonAsync("/shiip/public-api/v2/shipping-order/fee", ghnRequest);
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("GHN Fee API response: " + responseContent);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("GHN API failed: " + response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadFromJsonAsync<GHNFeeResponse>();
            var baseFee = json.Data.Total;

            var methods = new List<ShippingMethod>
            {
                new ShippingMethod { Name = "Tiêu chuẩn", Type = "standard", Fee = baseFee },
                new ShippingMethod { Name = "Nhanh", Type = "express", Fee = baseFee + 10000 }
            };

            // Priority only TP.HCM
            if (_hcmDistrictIds.Contains(toDistrictId.Value))
            {
                methods.Add(new ShippingMethod { Name = "Hỏa tốc", Type = "super-express", Fee = baseFee + 30000 });
            }

            return new ShippingMethodResponseDto { Methods = methods };
        }
    }
}