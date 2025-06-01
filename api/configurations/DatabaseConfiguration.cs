using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace api.configurations
{
    public static class DatabaseConfiguration
    {
        public static void ConfigurationMongoDb(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDb");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("MongoDB connection string is not configured");
            }
            services.AddDbContext<iTribeDbContext>(options =>
            {
                options.UseMongoDB(connectionString, "hufPhone");
            });
            Console.WriteLine("Connected MongoDb");
        }
    }
}