using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class ShippingDto
    {   
        [JsonPropertyName("shippingAddress")]
        public string ShippingAddress { get; set; } = string.Empty;
        [JsonPropertyName("height")]
        public int Height { get; set; }
        [JsonPropertyName("length")]
        public int Length { get; set; }
        [JsonPropertyName("width")]
        public int Width { get; set; }
        [JsonPropertyName("weight")]
        public int Weight { get; set; }
        [JsonPropertyName("insuranceValue")]
        public int InsuranceValue { get; set; }

    }

    public class GHNFeeResponse
    {
        public GHNFeeData Data { get; set; } = new();
    }

    public class GHNFeeData
    {
        public int Total { get; set; }
    }

    public class ShippingMethod
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Fee { get; set; }
    }
    public class ShippingMethodResponseDto
    {
        public List<ShippingMethod> Methods { get; set; } = new();
    }
    public class GHNProvinceResponse
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<Province> Data { get; set; }

    }

    public class Province
    {
        public int ProvinceID { get; set; }
        public string ProvinceName { get; set; } = string.Empty;
    }
    public class GHNDistrictResponse
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<District> Data { get; set; } = new();
    }

    public class District
    {
        public int DistrictID { get; set; }
        public string DistrictName { get; set; } = string.Empty;
        public int ProvinceID { get; set; }
    }

    public class GHNWardResponse
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<Ward> Data { get; set; } = new();
    }

    public class Ward
    {
        public string WardCode { get; set; } = string.Empty;
        public string WardName { get; set; } = string.Empty;
        public int DistrictID { get; set; }
    }

}