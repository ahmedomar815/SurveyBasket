
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SurveyBasket.Services
{
    public class CacheService(IDistributedCache distributedCached) : ICacheService
    {
        private IDistributedCache _distributedCached = distributedCached;

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancallationToken = default) where T : class
        {
            var cacheValue= await _distributedCached.GetStringAsync(key, cancallationToken);
            return cacheValue is null ? null : JsonSerializer.Deserialize<T>(cacheValue);
        }
        public async Task SetAsync<T>(string Key, T value, CancellationToken cancallationToken = default) where T : class
        {
           await _distributedCached.SetStringAsync(Key, JsonSerializer.Serialize(value), cancallationToken);
           
        }
        public async Task RemoveAsync<T>(string Key, CancellationToken cancallationToken = default) where T : class
        {
           await _distributedCached.RemoveAsync(Key, cancallationToken);
        }

    
    }
}
