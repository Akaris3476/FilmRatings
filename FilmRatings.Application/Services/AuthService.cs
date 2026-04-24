using System.Security.Claims;
using FilmRatings.Core.Abstractions.Auth;
using FilmRatings.Core.Abstractions.Repositories;
using FilmRatings.Core.Abstractions.Services;
using FilmRatings.Core.Models;

namespace FilmRatings.Application.Services;

public class AuthService : IAuthService
{
	private readonly IJwtProvider _jwtProvider;
	private readonly IRefreshTokensRepository _refreshTokensRepository;
	private readonly TimeSpan _refreshTokenExpiration = TimeSpan.FromMinutes(30);

	public AuthService(IJwtProvider jwtProvider, IRefreshTokensRepository refreshTokensRepository)
	{
		_jwtProvider = jwtProvider;
		_refreshTokensRepository = refreshTokensRepository;
	}

	public ClaimsPrincipal GetClaimsFromExpiredToken(string accessToken)
	{
		var claims = _jwtProvider.RetrieveClaimsFromExpiredToken(accessToken);
		return claims;
	}

	public async Task<RefreshToken> GetRefreshToken(string refreshToken)
	{
		RefreshToken refreshTokenObj = await _refreshTokensRepository.Get(refreshToken);
		return refreshTokenObj;
		
	}

	public async Task DeleteRefreshToken(string refreshToken)
	{
		await _refreshTokensRepository.Delete(refreshToken);
	}
	
	public async Task RevokeRefreshTokensFromUser(Guid userId)
	{
		await _refreshTokensRepository.DeleteAll(userId);
	}

	public (string, string) GetNewTokens(ClaimsPrincipal accessTokenClaims)
	{
		var claims = ClaimService.ParseClaim(accessTokenClaims);
		
		if (claims.userId == null || claims.username == null)
			throw new Exception("Invalid Access token claims");
		
		string accessToken = _jwtProvider.GenerateToken((Guid)claims.userId, claims.username, claims.isAdmin );
		string refreshToken = _jwtProvider.GenerateRefreshTokenString();
		
		
		return (accessToken, refreshToken);
		
	}
	
	public Task SaveRefreshToken(string token, Guid userId)
	{
		RefreshToken refreshToken = new(Guid.NewGuid(), token, userId, DateTime.UtcNow.Add(_refreshTokenExpiration) );
		
		return _refreshTokensRepository.Add(refreshToken);
	}
}