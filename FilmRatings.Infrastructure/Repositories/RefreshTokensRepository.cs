using FilmRatings.Core.Abstractions.Repositories;
using FilmRatings.Core.Models;
using FilmRatings.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmRatings.Infrastructure.Repositories;

public class RefreshTokensRepository : IRefreshTokensRepository
{
	private readonly FilmRatingsDbContext _dbContext;

	public RefreshTokensRepository(FilmRatingsDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<RefreshToken> Get(string refreshToken)
	{
		RefreshTokenEntity? tokenEntity = await _dbContext.RefreshTokens
			.Include(t => t.User)
			.FirstOrDefaultAsync(t => t.Token ==  refreshToken);

		if (tokenEntity == null)
			throw new KeyNotFoundException("Refresh token not found");

		
		RefreshToken token = new(tokenEntity.Id, tokenEntity.Token, tokenEntity.UserId, tokenEntity.ExpiresOnUTC);
		
		return token;
	}

	public async Task Add(RefreshToken refreshToken)
	{
		RefreshTokenEntity entity = new()
		{
			Id =  refreshToken.Id,
			Token = refreshToken.Token,
			UserId = refreshToken.UserId,
			ExpiresOnUTC = refreshToken.ExpiresOnUtc
		};
		
		await _dbContext.RefreshTokens
			.AddAsync(entity);
		await _dbContext.SaveChangesAsync();
		
	}

	public async Task Delete(string refreshToken)
	{
		await _dbContext.RefreshTokens
			.Where(t => t.Token == refreshToken)
			.ExecuteDeleteAsync();
	}
	
	public async Task DeleteAll(Guid userId)
	{
		await _dbContext.RefreshTokens
			.Where(t => t.UserId == userId)
			.ExecuteDeleteAsync();
	}
}