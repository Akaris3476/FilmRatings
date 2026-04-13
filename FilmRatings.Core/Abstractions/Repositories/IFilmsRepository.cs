using FilmRatings.Core.Models;

namespace FilmRatings.Core.Abstractions.Repositories;

public interface IFilmsRepository
{
	Task<List<Film>> GetAll(FilmsIncludeOptions includeOptions = FilmsIncludeOptions.None);
	Task<Guid> Create(Film film);
	Task<Guid> Update(Guid id, string title, string description);
	Task<Guid> Delete(Guid id);
	Task<Film> GetById(Guid id, FilmsIncludeOptions includeOptions = FilmsIncludeOptions.None);

}