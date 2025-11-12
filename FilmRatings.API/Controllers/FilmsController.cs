using FilmRatings.Contracts;
using FilmRatings.Core.Abstractions;
using FilmRatings.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmRatings.Controllers;

[ApiController]
[Route("/[controller]")]
public class FilmsController : ControllerBase
{
	private readonly IFilmsService _filmsService;

	public FilmsController(IFilmsService filmsService)
	{
		_filmsService = filmsService;
	}
	
	// [Authorize]
	[HttpGet]
	public async Task<ActionResult<List<FilmsResponse>>> GetFilms()
	{
		var films = await _filmsService.GetAllFilms();

		var response = films.Select(f => new FilmsResponse(f.Id, f.Title, f.Description));
		
		return Ok(response);
	}
	
	[HttpGet("all")]
	public async Task<ActionResult<List<FilmWithRatingsResponse>>> GetFilmsWithRatings( IRatingsService ratingsService)
	{
		var films = await _filmsService.GetAllFilms();

		films = await _filmsService.AddRatingsToFilms(films, ratingsService);

		var responses = films
			.Select(f => new FilmWithRatingsResponse(
				f.Id, f.Title, f.Description, f.AverageRating(),
				f.Ratings.Select(r => new RatingsResponse(r.Id, r.Value))
					.ToList()
				)
			);
		
		return Ok(responses);
	}

	
	[HttpGet("{filmId}")]
	public async Task<ActionResult<FilmsResponse>> GetFilm(Guid filmId)
	{
		
		 var film = await _filmsService.GetFilm(filmId);
		 var response = new FilmsResponse(film.Id, film.Title, film.Description);

		 return response;

	}
	
	[HttpGet("all/{filmId}")]
	public async Task<ActionResult<FilmWithRatingsResponse>> GetFilmWithRatings(Guid filmId, IRatingsService ratingsService)
	{
		var film = await _filmsService.GetFilm(filmId);
		var ratings = await ratingsService.GetAllRatings(film);
		
		var ratingsResponse = ratings.Select(r => new RatingsResponse(r.Id, r.Value)).ToList(); 
		var response = new FilmWithRatingsResponse(film.Id, film.Title, film.Description, Film.AverageRating(ratings), ratingsResponse);
		
		return Ok(response);
	}

	[HttpPost]
	public async Task<ActionResult<Guid>> CreateFilm([FromBody] FilmsRequest request)
	{

	
		var film = new Film(Guid.NewGuid(), request.Title, request.Description);
		Guid filmId = await _filmsService.AddFilm(film);
		return Ok(filmId);
			

	}

	[HttpPut("{id:guid}")]
	public async Task<ActionResult<Guid>> UpdateFilm(Guid id, [FromBody] FilmsRequest request)
	{
		
		//validation occures on creating film obj
		var film = new Film(id, request.Title, request.Description);
		
		var filmId = await _filmsService.UpdateFilm(id, request.Title, request.Description);
		return Ok(filmId);
	}
	
	[HttpDelete("{id:guid}")]
	public async Task<ActionResult<Guid>> DeleteFilm(Guid id)
	{
		var filmId = await _filmsService.DeleteFilm(id);
		return Ok(filmId);
	}
}