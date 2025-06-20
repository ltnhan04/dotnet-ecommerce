using CloudinaryDotNet;
using StackExchange.Redis;
using Stripe;

namespace api.configurations
{
    public static class ServiceConfiguration
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            ConfigureRedis(services);
            ConfigureCloudinary(services);
            ConfigureStripe();
        }

        private static void ConfigureRedis(IServiceCollection services)
        {
            var redisUrl = Environment.GetEnvironmentVariable("UPSTASH_REDIS_URL");

            if (string.IsNullOrEmpty(redisUrl))
            {
                throw new InvalidOperationException("Redis connection string is not configured");
            }

            var uri = new Uri(redisUrl);

            var userInfo = uri.UserInfo.Split(':');
            var username = userInfo[0];
            var password = userInfo.Length > 1 ? userInfo[1] : string.Empty;

            var config = new ConfigurationOptions
            {
                EndPoints = { $"{uri.Host}:{uri.Port}" },
                Ssl = true,
                User = username,
                Password = password,
                AbortOnConnectFail = false,
                ConnectTimeout = 10000,
            };

            var redis = ConnectionMultiplexer.Connect(config);
            services.AddSingleton<IConnectionMultiplexer>(redis);

            Console.WriteLine("✅ Connected to Upstash Redis");
        }


        private static void ConfigureCloudinary(IServiceCollection services)
        {
            var cloudName = Environment.GetEnvironmentVariable("CLOUDINARY_NAME");
            var apiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
            var apiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");

            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                throw new InvalidOperationException("Cloudinary credentials are not configured");
            }

            var account = new CloudinaryDotNet.Account(cloudName, apiKey, apiSecret);
            var cloudinary = new Cloudinary(account);
            services.AddSingleton(cloudinary);

            Console.WriteLine("✅ Connected to Cloudinary");
        }

        private static void ConfigureStripe()
        {
            var secretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("Stripe secret key is not configured");
            }

            StripeConfiguration.ApiKey = secretKey;
            Console.WriteLine("✅ Stripe configured");
        }
    }
}
