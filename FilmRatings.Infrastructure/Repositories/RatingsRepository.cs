using FilmRatings.Core.Abstractions;
using FilmRatings.Core.Models;
using FilmRatings.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace FilmRatings.Infrastructure.Repositories;

public class RatingsRepository : IRatingsRepository
{
	private readonly FilmRatingsDbContext _dbContext;
	private readonly IDistributedCache _distributedCache;

	public RatingsRepository(FilmRatingsDbContext dbContext, IDistributedCache distributedCache)
	{
		_dbContext = dbContext;
		_distributedCache = distributedCache;
	}

	public async Task<List<Rating>> GetAll(Film film)
	{
		
				
		string key = $"{nameof(Film)}-{film.Id}-AllRatings";
		
		string? cachedRatings = await _distributedCache.GetStringAsync(key);

		if (!string.IsNullOrEmpty(cachedRatings))
		{
			var cacheRatings = JsonConvert.DeserializeObject<List<Rating>>(cachedRatings);
			
			if (cacheRatings is not null) 
				return cacheRatings;
		}

		
		var ratingEntities = await _dbContext.Ratings
			.AsNoTracking()
			.Where(r => r.FilmId == film.Id)
			.ToListAsync();
		
		
		var ratings = ratingEntities
			.Select(ratingEntity => new Rating(ratingEntity.Id, ratingEntity.Value, film))
			.ToList();
		
		await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(ratings));
		
		return ratings;
	}

	public async Task<Rating> GetOne(Guid ratingId, Film film)
	{
		
		var ratingEntity = await _dbContext.Ratings
			.AsNoTracking()
			.FirstOrDefaultAsync(r => r.Id == ratingId);
		
		if (ratingEntity == null)
			throw new KeyNotFoundException($"Rating {ratingId} not found");
		
		var rating = new Rating(ratingEntity.Id, ratingEntity.Value, film);
		return rating;
		
	}

	public async Task<Guid> Create(Rating rating)
	{

		var ratingEntity = new RatingEntity
		{
			Id = rating.Id,
			FilmId = rating.FilmId,
			Value = rating.Value
		};
			
		await _dbContext.Ratings.AddAsync(ratingEntity);
		await _dbContext.SaveChangesAsync();
		
		_distributedCache.Remove($"{nameof(Film)}-{rating.FilmId}-AllRatings");

		return rating.Id;
	}

	public async Task<Guid> Update(Rating rating)
	{
		
		await _dbContext.Ratings
			.Where(r => r.Id == rating.Id)
			.ExecuteUpdateAsync(e => e
				.SetProperty(p => p.Value, p => rating.Value));
		
		_distributedCache.Remove($"{nameof(Film)}-{rating.FilmId}-AllRatings");

		return rating.Id;
		
	}
	
	
	public async Task<Guid> Delete(Guid id)
	{
		
		var ratingEntity = await _dbContext.Ratings
			.AsNoTracking()
			.FirstOrDefaultAsync(r => r.Id == id);
		
		if (ratingEntity is null)
			throw new KeyNotFoundException($"Rating {id} not found");
		
		_dbContext.Ratings.Remove(ratingEntity);
		_dbContext.SaveChanges();
		
		_distributedCache.Remove($"{nameof(Film)}-{ratingEntity.FilmId}-AllRatings");
		
		return id;
	}	
}