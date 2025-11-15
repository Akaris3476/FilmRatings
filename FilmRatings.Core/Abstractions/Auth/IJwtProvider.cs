namespace FilmRatings.Core.Abstractions.Auth;

public interface IJwtProvider
{
	string GenerateToken(Guid userId, bool isAdmin);
}