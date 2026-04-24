using FilmRatings.Application.Services;
using FilmRatings.Contracts;
using FilmRatings.Contracts.Ratings;
using FilmRatings.Core.Abstractions.Services;
using FilmRatings.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmRatings.Controllers;


[ApiController]
[Route("films/{filmId}/ratings")]
public class RatingsController : ControllerBase
{
	private readonly IRatingsService _ratingsService;

	public RatingsController(IRatingsService ratingsService)
	{
		_ratingsService = ratingsService;
	}
	
	
	[HttpGet]
	public async Task<ActionResult<PageResponse<RatingsResponse>>> GetRatings(
		Guid filmId, [FromQuery] int page = 1)
	{
		// var film = await _filmsService.GetFilm(filmId);
		var ratings = await _ratingsService.GetRatings(filmId, page);
		(int totalPages, int totalFilms) = await _ratingsService.GetRatingsCount(filmId);
		
		var ratingsResponse = ratings
			.Select(rating => new RatingsResponse(rating.Id, rating.UserId, rating.Username, rating.Value));
		
		
		
		var response = new PageResponse<RatingsResponse>(
			page, totalPages, totalFilms, ratingsResponse);
		
		return Ok(response);
	}
	
	
	[HttpPost]
	[Authorize]
	public async Task<ActionResult<Guid>> CreateRating(Guid filmId, [FromBody] RatingsRequest request)
	{
		string? userIdSerialized = User.FindFirst(UserClaims.UserId)?.Value;
		Guid? userId;
		
		if (userIdSerialized is not null)
			userId = Guid.Parse(userIdSerialized);
		else
			userId = null;
		
		string? username = User.FindFirst(UserClaims.Username)?.Value;
		
		var rating = new Rating(Guid.NewGuid(), request.Rating, filmId, userId, username);
		
		var ratingId = await _ratingsService.AddRating(rating);
		
		return Ok(ratingId);

	}

	[HttpPut("{ratingId:guid}")]
	[Authorize]
	public async Task<ActionResult<Guid>> UpdateRating(Guid filmId, Guid ratingId, [FromBody] RatingsRequest request)
	{

		(Guid? userId, string? username, bool isAdmin) = ClaimService.ParseClaim(User);

		bool ownsRating = await _ratingsService.IsRatingOwner(ratingId, userId);
		
		bool canAccessRating = ownsRating || isAdmin;
		if (!canAccessRating)
			return Forbid();
		
		
		var rating = new Rating(ratingId, request.Rating, filmId, userId, username);
		
		ratingId = await _ratingsService.UpdateRating(rating);
		
		return Ok(ratingId);
		
	}

	[HttpDelete("{ratingId:guid}")]
	[Authorize]
	public async Task<ActionResult<Guid>> DeleteRating(Guid ratingId)
	{
		(Guid? userId, _, bool isAdmin) = ClaimService.ParseClaim(User);

		bool ownsRating = await _ratingsService.IsRatingOwner(ratingId, userId);
		
		bool canAccessRating = ownsRating || isAdmin;
		if (!canAccessRating)
			return Forbid();


		ratingId = await _ratingsService.DeleteRating(ratingId);
		return Ok(ratingId);
		
	}
	
}