using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;

namespace RedisCacheApi.DataStore;

public class DataRepository
{
    private readonly IDistributedCache _cache;
    private readonly IDataStore _dataStore;

    public DataRepository(IDistributedCache cache, IDataStore dataStore)
    {
        _cache = cache;
        _dataStore = dataStore;
    }

    public async Task<TData> GetDataAsync<TData>(int id)
    {
        var cacheKey = $"data:{id}";
        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (cachedData != null)
        {
            Log.Logger.Information($"Getting data for ID: {id} from Caching.");
            return JsonSerializer.Deserialize<TData>(cachedData)!;
        }
        
        Log.Logger.Information($"Getting data for ID: {id} from Repository.");
        var data = await _dataStore.GetDataAsync();
        var serializedData = JsonSerializer.Serialize(data);
        await _cache.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) // Set your cache expiration
        });

        return (TData)(object)data;
    }

    public void InvalidateCache(int id)
    {
        var cacheKey = $"data:{id}";
        _cache.Remove(cacheKey);
    }
}