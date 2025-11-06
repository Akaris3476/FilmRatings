using FilmRatings.Core.Models;

namespace FilmRatings.Core.Abstractions;

public interface IRatingsRepository
{
	Task<List<Rating>> GetAll(Film film);
	Task<Guid> Create(Rating rating);
	Task<Guid> Update(Rating rating);
	Task<Guid> Delete(Guid id);
	Task<Rating> GetOne(Guid id, Film film);
}