using FilmRatings.Core.Models;

namespace FilmRatings.Core.Abstractions;

public interface IFilmsRepository
{
	Task<List<Film>> GetAll();
	Task<Guid> Create(Film film);
	Task<Guid> Update(Guid id, string title, string description);
	Task<Guid> Delete(Guid id);

	Task<Film> GetById(Guid id);

}