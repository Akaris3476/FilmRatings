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


	

	public async Task<List<Film>> GetFilms(string? title, int page = 1, string include = "")
	{
		title = title?.Trim();
		
		if (page < 1)
			throw new ArgumentException("Page must be greater than or equal to 1.");
		
		FilmsIncludeOptions includeOptions = ParseIncludeOptions(include);
		
		return await _filmsRepository.GetAll(title, page, includeOptions);

	}

	public async Task<(int totalPages, int totalFilms)> GetFilmsCount()
	{
		
		int totalFilms = await _filmsRepository.GetFilmsCount();
		int totalPages = (int)Math.Ceiling((double)totalFilms / _filmsRepository.PageSize);
 		return (totalPages, totalFilms);
	}
	
	public async Task<Film> GetFilm(Guid id, string include = "")
	{
		FilmsIncludeOptions includeOptions = ParseIncludeOptions(include);

		return await _filmsRepository.GetById(id, includeOptions);
	}
	
	private FilmsIncludeOptions ParseIncludeOptions(string include)
	{
		if (string.IsNullOrWhiteSpace(include))
			return FilmsIncludeOptions.None;
		
		FilmsIncludeOptions includeOptions = new();
		
		foreach (string s in include.Split(","))
		{
			if (!Enum.TryParse<FilmsIncludeOptions>(s, true, out var option)) continue;
			includeOptions |= option;
		}
		
		return includeOptions;
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