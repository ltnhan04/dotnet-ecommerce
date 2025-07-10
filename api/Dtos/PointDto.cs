using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class GetCustomerPointDto
    {
        public int points { get; set; }
    }

    public class GetCustomerVoucherDto
    {
        public List<VoucherDto> vouchers { get; set; } = new();
    }

    public class VoucherDto
    {
        public string _id { get; set; } = string.Empty;
        public string? customer { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
        public int discountAmount { get; set; }
        public int pointsUsed { get; set; }
        public string status { get; set; } = string.Empty;
        public DateTime validFrom { get; set; }
        public DateTime validTo { get; set; }
    }

    public class ExchangePointForVoucherDto
    {
        public int pointsToUse { get; set; }
    }

    public class ApplyVoucherDto
    {
        public string voucherCode { get; set; } = string.Empty;
        public int orderTotal { get; set; }
    }

    public class ApplyVoucherResponseDto
    {
        public string voucherCode { get; set; } = string.Empty;
        public int discountAmount { get; set; }
        public int discountedTotal { get; set; }
    }

    public class UpdateStatusVoucherDto
    {
        public string voucherCode { get; set; } = string.Empty;
        public string orderId { get; set; } = string.Empty;
    }

    public class PointVoucherDto
    {
        public string _id { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
        public string customer { get; set; } = string.Empty;
        public int discountAmount { get; set; }
        public int pointsUsed { get; set; }
        public string status { get; set; } = string.Empty;
        public string? usedOrder { get; set; }
        public DateTime validFrom { get; set; }
        public DateTime validTo { get; set; }
    }
    public class VoucherFreeShipDto
    {
        public string _id { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
        public string customer { get; set; } = string.Empty;
        public int discountAmount { get; set; }
        public int pointsUsed { get; set; }
        public string status { get; set; } = string.Empty;
        public string? usedOrder { get; set; }
        public DateTime validFrom { get; set; }
        public DateTime validTo { get; set; }
    }
}