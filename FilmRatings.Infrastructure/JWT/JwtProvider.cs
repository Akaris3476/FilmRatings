using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FilmRatings.Core.Abstractions.Auth;
using FilmRatings.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FilmRatings.Infrastructure.JWT;

public class JwtProvider : IJwtProvider
{
	private readonly JwtOptions _options;

	public JwtProvider(IOptions<JwtOptions> options)
	{
		_options = options.Value;
	}
	
	public string GenerateToken(Guid userId, string username, bool isAdmin)
	{
		
		Claim[] claims = 
		[
			new(UserClaims.UserId, userId.ToString()),
			new(UserClaims.IsAdmin, isAdmin.ToString()),
			new(UserClaims.Username, username)
		];
		
		var signingCredentials = new SigningCredentials(
			new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
			SecurityAlgorithms.HmacSha256);
		
		var token = new JwtSecurityToken(
			claims: claims,
			signingCredentials: signingCredentials,
			expires: DateTime.UtcNow.AddMinutes(_options.ExpirationInMinutes));
		
		var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
		
		return tokenValue;
	}

	public string GenerateRefreshTokenString()
	{
		string token = Guid.NewGuid().ToString();
		return token;
	}
	
	public ClaimsPrincipal RetrieveClaimsFromExpiredToken(string token)
	{
		
		var tokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateLifetime = false,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey))

		};

		var tokenHandler = new JwtSecurityTokenHandler();
    
		ClaimsPrincipal? claims = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

		return claims;
	}
}