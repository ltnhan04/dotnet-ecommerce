using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace api.database
{
    public static class MongoDb
    {
        public static IServiceCollection ConnectMongoDB(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("MongoDb");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("MongoDB connection string is not configured");
            }
            services.AddDbContext<iTribeDbContext>(options =>
            {
                options.UseMongoDB(connectionString, "hufPhone");
            });
            Console.WriteLine("Connected mongoDB successfully " + connectionString);
            return services;
        }
        
    }
}