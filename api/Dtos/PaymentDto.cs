using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class PaymentDto
    {
        public string orderId { get; set; } = string.Empty;
        public List<VariantPaymentDto> variants { get; set; } = new();
    }

    public class VariantPaymentDto
    {
        public string variant { get; set; } = string.Empty;
        public int quantity { get; set; }
    }

    public class ProductVariantsPaymentDto
    {
        public string name { get; set; } = string.Empty;
        public int price { get; set; }
        public int quantity { get; set; }
        public string image { get; set; } = string.Empty;
    }
}