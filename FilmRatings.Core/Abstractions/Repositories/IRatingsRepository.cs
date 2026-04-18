using FilmRatings.Core.Models;

namespace FilmRatings.Core.Abstractions.Repositories;

public interface IRatingsRepository
{
	int PageSize { get; }
	Task<int> GetRatingsCount(Guid filmId);
	Task<List<Rating>> GetAll(Guid filmId, int page);
	Task<Guid> Create(Rating rating);
	Task<Guid> Update(Rating rating);
	Task<Guid> Delete(Guid id);
	Task<Rating> GetOne(Guid id);
}