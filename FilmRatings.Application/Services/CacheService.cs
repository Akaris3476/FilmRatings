using FilmRatings.Core.Abstractions.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace FilmRatings.Application.Services;

public class CacheService : ICacheService
{

	private readonly IDistributedCache _distributedCache;
	
	public CacheService(IDistributedCache distributedCache)
	{
		_distributedCache = distributedCache;
	}

	public async Task<T?> GetAsync<T>(string key)
	{
		string? cachedJson = await _distributedCache.GetStringAsync(key);

		if (!string.IsNullOrEmpty(cachedJson))
		{
			T? cachedObjects = JsonConvert.DeserializeObject<T>(cachedJson);
			
			if (cachedObjects is not null)
				return cachedObjects;
		}
		
		return default;
	}

	public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
	{
		string serialized =  JsonConvert.SerializeObject(value);
		
		var options = new DistributedCacheEntryOptions()
			.SetSlidingExpiration(expiration ?? TimeSpan.FromHours(1));
		
		await _distributedCache.SetStringAsync(key, serialized, options);
	}
	
	public async Task RemoveAsync(string key)
	{
		await _distributedCache.RemoveAsync(key);
	}
	
}