using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RealEstatePlatform.Application.Interfaces.Services;
using StackExchange.Redis;

namespace RealEstatePlatform.Infrastructure.Services;

/// <summary>
/// Redis cache service implementation
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(IDistributedCache cache, IConnectionMultiplexer redis)
    {
        _cache = cache;
        _redis = redis;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        var cachedData = await _cache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrEmpty(cachedData))
            return null;

        return JsonConvert.DeserializeObject<T>(cachedData);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default) where T : class
    {
        var serializedData = JsonConvert.SerializeObject(value);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
        };

        await _cache.SetStringAsync(key, serializedData, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var endpoints = _redis.GetEndPoints();
        var server = _redis.GetServer(endpoints.First());

        var keys = server.Keys(pattern: $"{prefix}*").ToArray();

        if (keys.Length > 0)
        {
            await db.KeyDeleteAsync(keys);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await _cache.GetStringAsync(key, cancellationToken);
        return !string.IsNullOrEmpty(value);
    }
}
