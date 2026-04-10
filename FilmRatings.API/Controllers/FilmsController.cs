using FilmRatings.Contracts.Films;
using FilmRatings.Contracts.Ratings;
using FilmRatings.Core.Abstractions.Services;
using FilmRatings.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmRatings.Controllers;

[ApiController]
[Route("/films")]
public class FilmsController : ControllerBase
{
	private readonly IFilmsService _filmsService;

	public FilmsController(IFilmsService filmsService)
	{
		_filmsService = filmsService;
	}
	
	[HttpGet]
	public async Task<ActionResult<List<FilmsResponse>>> GetFilms([FromQuery] string include = "")
	{
		List<Film> films;
		
		films = await _filmsService.GetAllFilms( include);
		
		var response = films.Select(f => new FilmsResponse(
				f.Id, f.Title, f.Description, f.AverageRating(),
				f.Ratings.Select(r => new RatingsResponse(r.Id, r.Value))
					.ToList()
			)
		);
		
		return Ok(response);
	}
	

	
	[HttpGet("{filmId}")]
	public async Task<ActionResult<FilmsResponse>> GetFilm(Guid filmId)
	{
		
		 var film = await _filmsService.GetFilm(filmId);
		 var response = new FilmsResponse(film.Id, film.Title, film.Description);

		 return response;

	}
	
	[HttpGet("all/{filmId}")]
	public async Task<ActionResult<FilmsResponse>> GetFilmWithRatings(Guid filmId, IRatingsService ratingsService)
	{
		var film = await _filmsService.GetFilm(filmId);
		var ratings = await ratingsService.GetAllRatings(film);
		
		var ratingsResponse = ratings.Select(r => new RatingsResponse(r.Id, r.Value)).ToList(); 
		var response = new FilmsResponse(film.Id, film.Title, film.Description, Film.AverageRating(ratings), ratingsResponse);
		
		return Ok(response);
	}
	
	[Authorize(Policy="AdminPolicy")]
	[HttpPost]
	public async Task<ActionResult<Guid>> CreateFilm([FromBody] FilmsRequest request)
	{

	
		var film = new Film(Guid.NewGuid(), request.Title, request.Description);
		Guid filmId = await _filmsService.AddFilm(film);
		return Ok(filmId);
			

	}

	[Authorize(Policy="AdminPolicy")]
	[HttpPut("{filmId:guid}")]
	public async Task<ActionResult<Guid>> UpdateFilm(Guid filmId, [FromBody] FilmsRequest request)
	{
		
		//validation occurs on creating film obj
		var film = new Film(filmId, request.Title, request.Description);
		
		filmId = await _filmsService.UpdateFilm(filmId, request.Title, request.Description);
		return Ok(filmId);
	}
	
	[Authorize(Policy="AdminPolicy")]
	[HttpDelete("{filmId:guid}")]
	public async Task<ActionResult<Guid>> DeleteFilm(Guid filmId)
	{
		filmId = await _filmsService.DeleteFilm(filmId);
		return Ok(filmId);
	}
}