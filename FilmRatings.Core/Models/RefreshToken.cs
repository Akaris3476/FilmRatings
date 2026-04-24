namespace FilmRatings.Core.Models;

public class RefreshToken
{
	public Guid Id { get; set; }
	public string Token { get; set; } 
	public Guid UserId { get; set; }
	public DateTime ExpiresOnUtc { get; set; }

	public RefreshToken(Guid id, string token, Guid userId, DateTime expiresOnUtc)
	{
		Id = id;
		Token = token;
		UserId = userId;
		ExpiresOnUtc = expiresOnUtc;
	}


}