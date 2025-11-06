using FilmRatings.Core.Abstractions;
using FilmRatings.Core.Models;
using FilmRatings.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmRatings.DataAccess.Repositories;

public class RatingsRepository : IRatingsRepository
{
	private readonly FilmRatingsDbContext _dbContext;

	public RatingsRepository(FilmRatingsDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<List<Rating>> GetAll(Film film)
	{
		var ratingEntities = await _dbContext.Ratings
			.AsNoTracking()
			.Where(r => r.FilmId == film.Id)
			.ToListAsync();
		
		
		var ratings = ratingEntities
			.Select(ratingEntity => new Rating(ratingEntity.Id, ratingEntity.Value, film))
			.ToList();
		
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
		
		return rating.Id;
	}

	public async Task<Guid> Update(Rating rating)
	{
		
		await _dbContext.Ratings
			.Where(r => r.Id == rating.Id)
			.ExecuteUpdateAsync(e => e
				.SetProperty(p => p.Value, p => rating.Value));
		
		return rating.Id;
		
	}
	
	
	public async Task<Guid> Delete(Guid id)
	{
		await _dbContext.Ratings
			.Where(r => r.Id == id)
			.ExecuteDeleteAsync();
		
		return id;
	}	
}