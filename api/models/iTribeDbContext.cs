using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace api.models
{
    public class iTribeDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Chatbot> Chatbots { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<PointVoucher> PointVouchers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<User> Users { get; set; }
        public iTribeDbContext(DbContextOptions<iTribeDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToCollection("users");
            modelBuilder.Entity<ShippingMethod>().ToCollection("shippingmethods");
            modelBuilder.Entity<Review>().ToCollection("reviews");
            modelBuilder.Entity<ProductVariant>().ToCollection("productvariants");
            modelBuilder.Entity<Product>().ToCollection("points");
            modelBuilder.Entity<PointVoucher>().ToCollection("pointvouchers");
            modelBuilder.Entity<Point>().ToCollection("points");
            modelBuilder.Entity<Order>().ToCollection("orders");
            modelBuilder.Entity<FAQ>().ToCollection("fags");
            modelBuilder.Entity<Chatbot>().ToCollection("chatbots");
            modelBuilder.Entity<Category>().ToCollection("categories");
        }

    }
}