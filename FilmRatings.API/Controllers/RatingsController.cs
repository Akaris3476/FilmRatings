using FilmRatings.Contracts;
using FilmRatings.Core.Abstractions;
using FilmRatings.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace FilmRatings.Controllers;


[ApiController]
[Route("Films/{filmId}/[controller]")]
public class RatingsController : ControllerBase
{
	private readonly IRatingsService _ratingsService;
	private readonly IFilmsService _filmsService;

	public RatingsController(IRatingsService ratingsService, IFilmsService filmsService)
	{
		_ratingsService = ratingsService;
		_filmsService = filmsService;
	}
	
	
	[HttpGet]
	public async Task<ActionResult<List<RatingsResponse>>> GetRatings(Guid filmId)
	{
		var film = await _filmsService.GetFilm(filmId);
		var ratings = await _ratingsService.GetAllRatings(film);

		var ratingsResponse = ratings
			.Select(rating => new RatingsResponse(rating.Id, rating.Value));
		
		return Ok(ratingsResponse);
	}
	
	
	[HttpPost]
	public async Task<ActionResult<Guid>> CreateRating(Guid filmId, [FromBody] RatingsRequest request)
	{

		var film = await _filmsService.GetFilm(filmId);
		var rating = new Rating(Guid.NewGuid(), request.Rating, film);
		
		var ratingId = await _ratingsService.AddRating(rating);
		
		return Ok(ratingId);

	}

	[HttpPut("{ratingId:guid}")]
	public async Task<ActionResult<Guid>> UpdateRating(Guid filmId, Guid ratingId, [FromBody] RatingsRequest request)
	{
		var film = await _filmsService.GetFilm(filmId);
		var rating = new Rating(ratingId, request.Rating, film );
		
		ratingId = await _ratingsService.UpdateRating(rating);
		
		return Ok(ratingId);
		
	}

	[HttpDelete("{ratingId:guid}")]
	public async Task<ActionResult<Guid>> DeleteRating( Guid ratingId)
	{
		
		ratingId = await _ratingsService.DeleteRating(ratingId);
		return Ok(ratingId);
		
	}
		
}