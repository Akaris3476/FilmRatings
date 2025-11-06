using FilmRatings.Core.Models;

namespace FilmRatings.Core.Abstractions;

public interface IFilmsService : IAddRatingsToFilmsService
{
	
	Task<List<Film>> GetAllFilms();

	Task<Film> GetFilm(Guid id);

	Task<Guid> AddFilm(Film film);

	Task<Guid> UpdateFilm(Guid id, string title, string description);

	Task<Guid> DeleteFilm(Guid id);

}

public interface IAddRatingsToFilmsService
{
	Task<List<Film>> AddRatingsToFilms(List<Film> films, IRatingsService ratingsService);
}