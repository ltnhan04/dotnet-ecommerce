using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Interfaces
{
    public interface IRedisRepository
    {
        Task SetAsync(string key, string value, TimeSpan? expiry = null);
    }
}