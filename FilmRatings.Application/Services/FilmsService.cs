using FilmRatings.Core.Abstractions;
using FilmRatings.Core.Abstractions.Repositories;
using FilmRatings.Core.Abstractions.Services;
using FilmRatings.Core.Models;

namespace FilmRatings.Application.Services;

public class FilmsService : IFilmsService
{
	private readonly IFilmsRepository _filmsRepository;

	public FilmsService(IFilmsRepository filmsRepository)
	{
		_filmsRepository = filmsRepository;
	}


	public async Task<List<Film>> GetAllFilms(string? include)
	{
		if (string.IsNullOrEmpty(include)) return await _filmsRepository.GetAll(FilmsIncludeOptions.None);
		
		FilmsIncludeOptions includeOptions = ParseIncludeString(include);
		
		return await _filmsRepository.GetAll(includeOptions);

	}

	private FilmsIncludeOptions ParseIncludeString(string include)
	{
		FilmsIncludeOptions includeOptions = new();
		
		foreach (string s in include.Split(","))
		{
			if (!Enum.TryParse<FilmsIncludeOptions>(s, true, out var option)) continue;
			includeOptions |= option;
		}
		
		return includeOptions;
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

	// public async Task<List<Film>> AddRatingsToFilms(List<Film> films, IRatingsService ratingsService)
	// {
	// 	foreach (var film in films)
	// 	{
	// 		var ratings = await ratingsService.GetAllRatings(film);
	// 		film.SetRatingList(ratings);
	// 	}
	//
	// 	return films;
	// }
	
	
}