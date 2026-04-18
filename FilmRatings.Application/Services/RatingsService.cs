using FilmRatings.Core.Abstractions.Repositories;
using FilmRatings.Core.Abstractions.Services;
using FilmRatings.Core.Models;

namespace FilmRatings.Application.Services;

public class RatingsService : IRatingsService
{
	private readonly IRatingsRepository _ratingsRepository;

	public RatingsService(IRatingsRepository ratingsRepository)
	{
		_ratingsRepository = ratingsRepository;
	}

	public async Task<List<Rating>> GetRatings(Guid filmId, int page)
	{
		if (page < 1)
			throw new ArgumentException("Page must be greater than or equal to 1.");
		
		return await _ratingsRepository.GetAll(filmId, page);
	}
	
	public async Task<(int totalPages, int totalRatings)> GetRatingsCount(Guid filmId)
	{
		int totalRatings = await _ratingsRepository.GetRatingsCount(filmId);
		int totalPages = (int)Math.Ceiling((double) totalRatings / _ratingsRepository.PageSize);
		return (totalPages, totalRatings);
	}

	public async Task<Rating> GetOneRating(Guid ratingId)
	{
		return await _ratingsRepository.GetOne(ratingId);
	}
	
	public async Task<Guid> AddRating(Rating rating)
	{
		return await _ratingsRepository.Create(rating);
	}

	public async Task<Guid> UpdateRating(Rating rating)
	{
		return await _ratingsRepository.Update(rating);
	}
	
	public async Task<Guid> DeleteRating(Guid id)
	{
		return await _ratingsRepository.Delete(id);
	}

	public async Task<bool> IsRatingOwner(Guid ratingId, Guid? userId)
	{
		if (userId is null) return false;
		
		Rating rating = await _ratingsRepository.GetOne(ratingId);
		return rating.UserId == userId;
	}
	
}