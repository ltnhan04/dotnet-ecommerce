using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace api.database
{
    public static class Redis
    {
        public static IServiceCollection ConnectRedis(this IServiceCollection services, IConfiguration config)
        {
            var redisUrl = config.GetConnectionString("Redis");
            if (string.IsNullOrWhiteSpace(redisUrl))
            {
                throw new InvalidOperationException("Redis connection string is not configured");
            }
            var options = ConfigurationOptions.Parse(redisUrl);
            options.Ssl = true;
            options.AbortOnConnectFail = false;
            var redis = ConnectionMultiplexer.Connect(options);
            services.AddSingleton<IConnectionMultiplexer>(redis);
            Console.WriteLine("Connected redis successfully " + redisUrl);
            return services;
        }
    }
}