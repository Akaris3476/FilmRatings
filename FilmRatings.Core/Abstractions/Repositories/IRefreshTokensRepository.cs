using FilmRatings.Core.Models;

namespace FilmRatings.Core.Abstractions.Repositories;

public interface IRefreshTokensRepository
{
	Task<RefreshToken> Get(string refreshToken);
	Task DeleteAll(Guid userId);
	Task Add(RefreshToken refreshToken);
	Task Delete(string refreshToken);

}