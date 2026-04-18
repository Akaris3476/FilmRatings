using FilmRatings.Core.Models;

namespace FilmRatings.Core.Abstractions.Repositories;

public interface IFilmsRepository
{
	int PageSize { get; }
	Task<int> GetFilmsCount();
	Task<List<Film>> GetAll(string? title, int page, FilmsIncludeOptions includeOptions);
	Task<Guid> Create(Film film);
	Task<Guid> Update(Guid id, string title, string description);
	Task<Guid> Delete(Guid id);
	Task<Film> GetById(Guid id, FilmsIncludeOptions includeOptions);

}