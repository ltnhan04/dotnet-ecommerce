using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class ShippingDto
    {
        public string ShippingAddress { get; set; }
        public int Height { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Weight { get; set; }
        public int InsuranceValue { get; set; }

    }

    public class GHNFeeResponse
    {
        public GHNFeeData Data { get; set; }
    }

    public class GHNFeeData
    {
        public int Total { get; set; }
    }

    public class ShippingMethod
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Fee { get; set; }
    }
    public class ShippingMethodResponseDto
    {
        public List<ShippingMethod> Methods { get; set; }
    }
    public class GHNProvinceResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<Province> Data { get; set; }

    }

    public class Province
    {
        public int ProvinceID { get; set; }
        public string ProvinceName { get; set; }
    }
    public class GHNDistrictResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<District> Data { get; set; }
    }

    public class District
    {
        public int DistrictID { get; set; }
        public string DistrictName { get; set; }
        public int ProvinceID { get; set; }
    }

    public class GHNWardResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<Ward> Data { get; set; }
    }

    public class Ward
    {
        public string WardCode { get; set; }
        public string WardName { get; set; }
        public int DistrictID { get; set; }
    }

}