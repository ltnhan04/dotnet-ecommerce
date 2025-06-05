using api.Interfaces;
using StackExchange.Redis;

namespace api.Repositories
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IConnectionMultiplexer _redis;
        public RedisRepository(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }
        public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            var db = _redis.GetDatabase();
            await db.StringSetAsync(key, value, expiry);
        }
        public async Task<string?> GetAsync(string key)
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }
        public async Task DeleteAsync(string key)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(key);
        }
        public async Task<long> IncrementAsync(string key)
        {
            var db = _redis.GetDatabase();
            return await db.StringIncrementAsync(key);
        }
    }
}