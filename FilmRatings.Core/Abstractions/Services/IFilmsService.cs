using FilmRatings.Core.Models;

namespace FilmRatings.Core.Abstractions.Services;

public interface IFilmsService 
{
	
	Task<List<Film>> GetAllFilms(string include = "", string? title = null);
	Task<Film> GetFilm(Guid id, string include = "");

	Task<Guid> AddFilm(Film film);

	Task<Guid> UpdateFilm(Guid id, string title, string description);

	Task<Guid> DeleteFilm(Guid id);
	// Task<List<Film>> AddRatingsToFilms(List<Film> films, IRatingsService ratingsService);

}