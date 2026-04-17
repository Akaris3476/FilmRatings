using System.Security.Claims;
using FilmRatings.Core.Models;

namespace FilmRatings.Application.Services;

public static class ClaimService 
{
	
	public static (Guid? userId, string? username, bool isAdmin) ParseClaim(ClaimsPrincipal user)
	{
		Guid? userId = GetUserId(user);
		string? username = GetUsername(user);
		bool isAdmin = IsAdmin(user);
		
		return (userId, username, isAdmin);
	}

	private static Guid? GetUserId(ClaimsPrincipal user)
	{
		string? userIdSerialized = user.FindFirst(UserClaims.UserId)?.Value;
		Guid? userId;
		
		if (userIdSerialized is not null)
			userId = Guid.Parse(userIdSerialized);
		else
			userId = null;

		return userId;
	}
	
	private static string? GetUsername(ClaimsPrincipal user) 
		=> user.FindFirst(UserClaims.Username)?.Value;

	private static bool IsAdmin(ClaimsPrincipal user)
	{
		string? isAdmin = user.FindFirst(UserClaims.IsAdmin)?.Value;
		
		return isAdmin == "True";
	}
	
	
}