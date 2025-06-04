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
    }
}