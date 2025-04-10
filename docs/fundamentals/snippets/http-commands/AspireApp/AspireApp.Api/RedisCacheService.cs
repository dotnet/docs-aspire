using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace AspireApp.Api;

public sealed class RedisCacheService(IConnectionMultiplexer connection) : ICacheService
{
    private readonly IDatabase _db = connection.GetDatabase();
    private const string RegistryKey = "cache:registry";

    public async Task<T?> GetAsync<T>(string key, JsonTypeInfo<T> jsonTypeInfo)
    {
        var value = await _db.StringGetAsync(key);

        return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize(value!, jsonTypeInfo);
    }

    public async Task SetAsync<T>(string key, T value, JsonTypeInfo<T> jsonTypeInfo, TimeSpan? absoluteExpiration = null)
    {
        var json = JsonSerializer.Serialize(value, jsonTypeInfo);

        await _db.StringSetAsync(key, json, absoluteExpiration ?? TimeSpan.FromMinutes(5));
        await _db.SetAddAsync(RegistryKey, key);
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
        await _db.SetRemoveAsync(RegistryKey, key);
    }

    public async IAsyncEnumerable<string> GetRegisteredKeysAsync()
    {
        var keys = await _db.SetMembersAsync(RegistryKey);

        foreach (var key in keys)
        {
            yield return key.ToString();
        }
    }

    public async Task ClearAllAsync()
    {
        await foreach (var key in GetRegisteredKeysAsync())
        {
            await _db.KeyDeleteAsync(key);
        }

        await _db.KeyDeleteAsync(RegistryKey);
    }
}
