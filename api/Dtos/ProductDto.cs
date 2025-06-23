using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace api.Dtos
{
    public class ProductDto
    {
        public string _id { get; set; } = string.Empty;
        public CategoryDto category { get; set; } = new();
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public List<VariantDto> variants = new();
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;
    }
    public class CategoryDto
    {
        public string _id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
    }
    public class ColorDto
    {
        public string colorName { get; set; } = string.Empty;
        public string colorCode { get; set; } = string.Empty;
    }
    public class VariantDto
    {
        public string _id { get; set; } = string.Empty;
        public ColorDto color { get; set; } = new();
        public string product { get; set; } = string.Empty;
        public int rating { get; set; } = 0;
        public string storage { get; set; } = string.Empty;
        public int price { get; set; } = 0;
        public int stock_quantity { get; set; } = 0;
        public string slug { get; set; } = string.Empty;
        public List<string> images { get; set; } = new();
        public List<ReviewDto> reviews { get; set; } = new();

    }
    public class ReviewDto
    {
        public string _id { get; set; } = string.Empty;
        public string variant { get; set; } = string.Empty;
        public string user { get; set; } = string.Empty;
        public int rating { get; set; } = 0;
        public string comment { get; set; } = string.Empty;
    }
    public class CreateProductDto
    {
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
    }
    public class UpdateProductDto
    {
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
    }
}