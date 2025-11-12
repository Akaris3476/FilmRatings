using FilmRatings.Core.Models;

namespace FilmRatings.Core.Abstractions;

public interface IRatingsService
{
	Task<List<Rating>> GetAllRatings(Film film);
	Task<Guid> AddRating(Rating rating);
	Task<Guid> UpdateRating(Rating rating);
	Task<Guid> DeleteRating(Guid id);
	Task<Rating> GetOneRating(Guid ratingId, Film film);
}