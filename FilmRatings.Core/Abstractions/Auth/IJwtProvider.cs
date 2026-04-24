using System.Security.Claims;

namespace FilmRatings.Core.Abstractions.Auth;

public interface IJwtProvider
{
	string GenerateToken(Guid userId, string username, bool isAdmin);
	string GenerateRefreshTokenString();
	ClaimsPrincipal RetrieveClaimsFromExpiredToken(string token);
}