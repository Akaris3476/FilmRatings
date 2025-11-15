using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FilmRatings.Core.Abstractions.Auth;
using FilmRatings.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FilmRatings.Infrastructure;


public class JwtProvider : IJwtProvider
{
	private readonly JwtOptions _options;

	public JwtProvider(IOptions<JwtOptions> options)
	{
		_options = options.Value;
	}
	
	public string GenerateToken(Guid userId, bool isAdmin)
	{
		
		Claim[] claims = 
		[
			new("userId", userId.ToString()),
			new("Admin", isAdmin.ToString())
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
}