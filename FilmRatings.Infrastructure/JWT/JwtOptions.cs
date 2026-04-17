namespace FilmRatings.Infrastructure.JWT;

public class JwtOptions
{
	public string SecretKey { get; set; } = string.Empty;
	public int ExpirationInMinutes { get; set; } 
}