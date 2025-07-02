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
        public List<VariantDto> variants { get; set; } = new();
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
    public class CategoryDto
    {
        public string _id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string? parent_category { get; set; }
    }
    public class CreateCategoryDto
    {
        public string? _id { get; set; }
        public string name { get; set; } = string.Empty;
        public string? parent_category { get; set; }
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
        public string status { get; set; } = "in_stock";
        public int stock_quantity { get; set; } = 0;
        public string slug { get; set; } = string.Empty;
        public List<string> images { get; set; } = new();
        public List<ReviewDto> reviews { get; set; } = new();

    }
    public class ReviewDto
    {
        public string _id { get; set; } = string.Empty;
        public string variant { get; set; } = string.Empty;
        public int rating { get; set; } = 0;
        public string comment { get; set; } = string.Empty;
    }

    public class CreateReviewDto
    {
        public string _id { get; set; } = string.Empty;
        public string variant { get; set; } = string.Empty;
        public int rating { get; set; } = 0;
        public string comment { get; set; } = string.Empty;
    }

    public class UpdateReviewDto
    {
        public string _id { get; set; } = string.Empty;

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
        public string? category { get; set; }
    }
    // public class TopProductDto
    // {
    //     public ObjectId variantId { get; set; }
    //     // public string productName { get; set; } = string.Empty;
    //     public int totalSold { get; set; }
    //     // public string image { get; set; } = string.Empty;
    //     // public int price { get; set; }
    // }
    public class ProductSlug
    {
        public string slug { get; set; } = string.Empty;
    }
    public class TopProductDto
    {
        public ObjectId variantId { get; set; }
        public int totalSold { get; set; }
    }

    public class TopProductDtoRes
    {
        public string variantId { get; set; } = string.Empty;
        public string productId { get; set; } = string.Empty;
        public string productName { get; set; } = string.Empty;
        public string image { get; set; } = string.Empty;
        public int price { get; set; } = 0;
        public int totalSold { get; set; } = 0;
    }
    public class TopSalesByLocationDto
    {
        public string city { get; set; } = string.Empty;
        public int totalSold { get; set; }
    }
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
    }
}