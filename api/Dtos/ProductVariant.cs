using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stripe;

namespace api.Dtos
{
    public class CreateProductVariantDto
    {
        public string product { get; set; } = string.Empty;
        public string colorName { get; set; } = string.Empty;
        public string colorCode { get; set; } = string.Empty;
        public string storage { get; set; } = string.Empty;
        public int price { get; set; } = 0;
        public int stock_quantity { get; set; } = 0;
        public string name { get; set; } = string.Empty;
        public string slug { get; set; } = string.Empty;
        public List<IFormFile> files { get; set; } = new();
    }
    public class UpdateProductVariantDto : CreateProductVariantDto
    {
        public string variantId { get; set; } = string.Empty;
        public List<IFormFile> existingImages { get; set; } = new();
    }
    public class ProductVariantDto : CreateProductVariantDto
    {
        public string _id { get; set; } = string.Empty;
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}