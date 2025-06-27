using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        public string slug { get; set; } = string.Empty;
        [MaxLength(5)]
        public List<IFormFile> images { get; set; } = new();

    }
    public class UpdateProductVariantDto : CreateProductVariantDto
    {
        public string variantId { get; set; } = string.Empty;
        public List<string> existingImages { get; set; } = new();
    }
    public class ProductVariantDto
    {
        public string _id { get; set; } = string.Empty;

        public string product { get; set; } = string.Empty;
        public string colorName { get; set; } = string.Empty;
        public string colorCode { get; set; } = string.Empty;
        public string storage { get; set; } = string.Empty;
        public int price { get; set; } = 0;
        public int stock_quantity { get; set; } = 0;
        public string slug { get; set; } = string.Empty;
        public List<string> images { get; set; } = new();
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}