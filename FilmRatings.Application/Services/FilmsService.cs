using FilmRatings.Core.Abstractions;
using FilmRatings.Core.Models;

namespace FilmRatings.Application.Services;

public class FilmsService : IFilmsService
{
	private readonly IFilmsRepository _filmsRepository;

	public FilmsService(IFilmsRepository filmsRepository)
	{
		_filmsRepository = filmsRepository;
	}

	public async Task<List<Film>> GetAllFilms()
	{
		return await _filmsRepository.GetAll();
	}
	
	public async  Task<Film> GetFilm(Guid id)
	{
		return await _filmsRepository.GetById(id);
	}

	public async Task<Guid> AddFilm(Film film)
	{
		return await _filmsRepository.Create(film);
	}

	public async Task<Guid> UpdateFilm(Guid id, string title, string description)
	{
		return await _filmsRepository.Update(id, title, description);
	}

	public async Task<Guid> DeleteFilm(Guid id)
	{
		return await _filmsRepository.Delete(id);
	}

	public async Task<List<Film>> AddRatingsToFilms(List<Film> films, IRatingsService ratingsService)
	{
		foreach (var film in films)
		{
			var ratings = await ratingsService.GetAllRatings(film);
			film.SetRatingList(ratings);
		}

		return films;
	}
	
	
}