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

	public async Task<List<Rating>> GetAllRatings(Film film)
	{
		return await _ratingsRepository.GetAll(film);
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