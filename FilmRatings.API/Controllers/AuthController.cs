using System.Security.Claims;
using FilmRatings.Contracts.Auth;
using FilmRatings.Core.Abstractions.Services;
using FilmRatings.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace FilmRatings.Controllers;

[ApiController]
[Route("auth")]
public class AuthController  : ControllerBase
{
	private readonly IUsersService _usersService;
	private readonly IAuthService _authService;

	public AuthController(IUsersService usersService, IAuthService authService)
	{
		_usersService = usersService;
		_authService = authService;
	}
	
	
	[HttpPost("register")]
	public async Task<ActionResult> Register([FromBody]RegisterUserRequest request)
	{
		
		await _usersService.Register(request.username, request.email, request.password);
		
		return Ok();
			
	}

	
	[HttpPost("login")]
	public async Task<ActionResult<string>> Login([FromBody]LoginUserRequest request)
	{
	
		var (accessToken, refreshToken) = await _usersService.Login(request.Email, request.Password);
		
		var cookieOptions = new CookieOptions 
		{ 
			HttpOnly = true, 
			Secure = true, 
			SameSite = SameSiteMode.Strict 
		};
		HttpContext.Response.Cookies.Append("AccessToken", accessToken, cookieOptions); 
		HttpContext.Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);	
		
		return Ok((accessToken, refreshToken));
		
	}

	[HttpPost("refresh")]
	public async Task<ActionResult<string>> Refresh()
	{
		
		if (!Request.Cookies.TryGetValue("AccessToken", out var accessToken) ||
		    !Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
		{
			return Unauthorized("Tokens are missing");
		}
		
		var claims = _authService.GetClaimsFromExpiredToken(accessToken);

		try
		{
			string? userIdSerialized = claims.FindFirst(UserClaims.UserId)?.Value;
			if (!Guid.TryParse(userIdSerialized, out var userId))
				return Unauthorized("Invalid Access Token");

			RefreshToken refreshTokenFromDb = await _authService.GetRefreshToken(refreshToken);

			if (refreshTokenFromDb.UserId != userId)
			{
				await _authService.RevokeRefreshTokensFromUser(userId);
				await _authService.RevokeRefreshTokensFromUser(refreshTokenFromDb.UserId);
				return Unauthorized("Access token does not correspond to refresh token. All sessions terminated");
			}
			
			if (refreshTokenFromDb.ExpiresOnUtc < DateTime.UtcNow)
			{
				await _authService.DeleteRefreshToken(refreshToken);
				return Unauthorized("Expired Refresh Token");
			}
			
			await _authService.DeleteRefreshToken(refreshToken);
			
			(accessToken, refreshToken) = _authService.GetNewTokens(claims);
			
			await _authService.SaveRefreshToken(refreshToken, userId);
			
			var cookieOptions = new CookieOptions 
			{ 
				HttpOnly = true, 
				Secure = true, 
				SameSite = SameSiteMode.Strict 
			};
			HttpContext.Response.Cookies.Append("AccessToken", accessToken, cookieOptions); 
			HttpContext.Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);	
			return Ok((accessToken, refreshToken));
		}
		catch (Exception e)
		{
			return Unauthorized(e.Message);
		}
		
	}

	[HttpPost("logout")]
	public async Task<ActionResult> Logout()
	{
		if (!Request.Cookies.TryGetValue("AccessToken", out var accessToken) ||
		    !Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
		{
			return NoContent();
		}
		
		ClaimsPrincipal claims = _authService.GetClaimsFromExpiredToken(accessToken!);
		var userIdSerialized = claims.FindFirst(UserClaims.UserId)?.Value;

		if (Guid.TryParse(userIdSerialized, out Guid userId))
		{
			await _authService.RevokeRefreshTokensFromUser(userId);
		}
		
		Response.Cookies.Delete("AccessToken");
		Response.Cookies.Delete("RefreshToken");
		
		return NoContent();
	}
		
		
}