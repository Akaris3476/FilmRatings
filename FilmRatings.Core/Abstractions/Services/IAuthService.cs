using System.Security.Claims;
using FilmRatings.Core.Models;

namespace FilmRatings.Core.Abstractions.Services;

public interface IAuthService
{
	ClaimsPrincipal GetClaimsFromExpiredToken(string accessToken);
	Task<RefreshToken> GetRefreshToken(string refreshToken);
	Task RevokeRefreshTokensFromUser(Guid userId);
	(string, string) GetNewTokens(ClaimsPrincipal accessTokenClaims);
	Task SaveRefreshToken(string refreshToken, Guid userId);
	Task DeleteRefreshToken(string refreshToken);
}