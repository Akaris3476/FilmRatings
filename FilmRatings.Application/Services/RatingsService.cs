using FilmRatings.Core.Abstractions;
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

	public async Task<Rating> GetOneRating(Guid ratingId, Film film)
	{
		return await _ratingsRepository.GetOne(ratingId, film);
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
	
}