using FilmRatings.Core.Models;

namespace FilmRatings.Core.Abstractions.Repositories;

public interface IFilmsRepository
{
	Task<Guid> Create(Film film);
	Task<Guid> Update(Guid id, string title, string description);
	Task<Guid> Delete(Guid id);
	Task<List<Film>> GetAll(FilmsIncludeOptions includeOptions);

	Task<Film> GetById(Guid id);

}