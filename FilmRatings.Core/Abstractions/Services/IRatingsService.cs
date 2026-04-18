using FilmRatings.Core.Models;

namespace FilmRatings.Core.Abstractions.Services;

public interface IRatingsService
{
	Task<List<Rating>> GetRatings(Guid filmId, int page);
	Task<Guid> AddRating(Rating rating);
	Task<Guid> UpdateRating(Rating rating);
	Task<Guid> DeleteRating(Guid id);
	Task<Rating> GetOneRating(Guid ratingId);
	Task<bool> IsRatingOwner(Guid ratingId, Guid? userId);
	Task<(int totalPages, int totalRatings)> GetRatingsCount(Guid filmId);
}