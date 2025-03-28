using System.Text.Json.Serialization.Metadata;

namespace AspireApp.Api;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, JsonTypeInfo<T> jsonTypeInfo);
    Task SetAsync<T>(string key, T value, JsonTypeInfo<T> jsonTypeInfo, TimeSpan? absoluteExpiration = null);
    Task RemoveAsync(string key);
    IAsyncEnumerable<string> GetRegisteredKeysAsync();
    Task ClearAllAsync();
}

