using FilmRatings.Core.Abstractions.Repositories;
using FilmRatings.Core.Models;
using FilmRatings.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmRatings.Infrastructure.Repositories;

public class RatingsRepository : IRatingsRepository
{
	private readonly FilmRatingsDbContext _dbContext;
	private readonly ICacheService _cacheService;
	
	private readonly TimeSpan _ratingCacheDuration = TimeSpan.FromMinutes(5);
	private readonly TimeSpan _allRatingsCacheDuration = TimeSpan.FromMinutes(5);
	private readonly TimeSpan _ratingsCountDuration = TimeSpan.FromMinutes(5);

	public int PageSize => 5;
	
	public RatingsRepository(FilmRatingsDbContext dbContext, ICacheService cacheService)
	{
		_dbContext = dbContext;
		_cacheService = cacheService;
	}
	
	public async Task<List<Rating>> GetAll(Guid filmId, int page)
	{
		
				
		string key = $"{nameof(Film)}-{filmId}-AllRatings-page-{page}";
		
		List<Rating>? cachedRatings = await _cacheService.GetAsync<List<Rating>>(key);
		
		if (cachedRatings is not null) 
			return cachedRatings;

		int ratingsToSkip = (page - 1) * PageSize;
		
		var ratingEntities = await _dbContext.Ratings
			.AsNoTracking()
			.OrderBy(r => r.Id)
			.Where(r => r.FilmId == filmId)
			.Skip(ratingsToSkip)
			.Take(PageSize)
			.Include(ratingEntity => ratingEntity.User)
			.ToListAsync();
		
		
		var ratings = ratingEntities
			.Select(ratingEntity => new Rating(ratingEntity.Id, ratingEntity.Value, filmId, ratingEntity.UserId, ratingEntity.User?.Username))
			.ToList();
		
		await _cacheService.SetAsync(key, ratings,  _allRatingsCacheDuration);
		
		return ratings;
	}
	
	public async Task<int> GetRatingsCount(Guid filmId)
	{
		string key = $"{nameof(Film)}-{nameof(GetRatingsCount)}";
		int? cachedCount = await _cacheService.GetAsync<int?>(key);
		
		if (cachedCount is not null)
			return cachedCount.Value;
		
		int count = await _dbContext.Ratings
			.Where(r => r.FilmId == filmId)
			.CountAsync();
		
		await _cacheService.SetAsync(key, count, _ratingsCountDuration);
		
		return count;
	}


	public async Task<Rating> GetOne(Guid ratingId)
	{
		
		var ratingEntity = await _dbContext.Ratings
			.AsNoTracking()
			.Include(ratingEntity => ratingEntity.User)
			.FirstOrDefaultAsync(r => r.Id == ratingId);
		
		if (ratingEntity == null)
			throw new KeyNotFoundException($"Rating {ratingId} not found");
		
		var rating = new Rating(ratingEntity.Id, ratingEntity.Value, ratingEntity.FilmId, ratingEntity.UserId, ratingEntity.User?.Username);
		return rating;
		
	}

	public async Task<Guid> Create(Rating rating)
	{

		var ratingEntity = new RatingEntity
		{
			Id = rating.Id,
			FilmId = rating.FilmId,
			Value = rating.Value,
			UserId = rating.UserId,
			Username =  rating.Username
		};
			
		await _dbContext.Ratings.AddAsync(ratingEntity);
		await _dbContext.SaveChangesAsync();
		
		await InvalidateCache(rating.FilmId);

		return rating.Id;
	}

	public async Task<Guid> Update(Rating rating)
	{
		
		await _dbContext.Ratings
			.Where(r => r.Id == rating.Id)
			.ExecuteUpdateAsync(e => e
				.SetProperty(p => p.Value, p => rating.Value));
		
		await InvalidateCache(rating.FilmId);
		
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
		await _dbContext.SaveChangesAsync();
		
		await InvalidateCache(ratingEntity.FilmId);
			
		return id;
	}

	private async Task InvalidateCache(Guid filmId)
	{
		await _cacheService.RemoveAsync($"{nameof(Film)}-{nameof(GetRatingsCount)}");
		
		foreach (var includeOption in Enum.GetValues<FilmsIncludeOptions>())
		{
			await _cacheService.RemoveAsync($"all-films-include-{(int)includeOption}");
		}
		
		int filmCount = await GetRatingsCount(filmId);
		int pageCount = (int)Math.Ceiling((double)filmCount / PageSize);
		
		for (int page = 0; page < pageCount; page++)
		{
			await _cacheService.RemoveAsync($"{nameof(Film)}-{filmId}-AllRatings-page-{page+1}");
		}
		
	}
}