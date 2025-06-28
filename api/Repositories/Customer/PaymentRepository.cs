using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.models;
using api.Utils;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using Stripe.Checkout;

namespace api.Repositories.Customer
{
    public class PaymentRepository
    {
        private readonly iTribeDbContext _context;
        public PaymentRepository(iTribeDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateCheckoutSession(string orderId, List<VariantPaymentDto> variants)
        {
            if (variants == null || variants.Count == 0)
            {
                throw new AppException("Invalid product variants", 400);
            }

            var variant = new List<ProductVariantsPaymentDto>();
            foreach (var item in variants)
            {
                var productVariants = await _context.ProductVariants
                    .FirstOrDefaultAsync(pv => pv._id == ObjectId.Parse(item.variant)) ?? throw new AppException($"Product variants {item.variant} not found", 404);

                var products = await _context.Products.ToListAsync();
                var product = products.FirstOrDefault(p => p._id == ObjectId.Parse(productVariants.product.ToString()));

                variant.Add(new ProductVariantsPaymentDto
                {
                    name = product.name,
                    price = productVariants.price,
                    quantity = item.quantity,
                    image = productVariants.images.FirstOrDefault()! ?? "null"
                });
            }
            
            return "sad";
        }
    }
}