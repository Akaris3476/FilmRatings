using FilmRatings.Core.Abstractions.Repositories;
using FilmRatings.Core.Models;
using FilmRatings.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmRatings.Infrastructure.Repositories;

public class UsersRepository : IUsersRepository
{
	private readonly FilmRatingsDbContext _dbContext;

	public UsersRepository(FilmRatingsDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<Guid> Add(User user)
	{
		var userEntity = new UserEntity
		{
			Email = user.Email,
			Username = user.Username,
			HashedPassword = user.HashedPassword,
			Id = user.Id,
			IsAdmin = user.IsAdmin
		};
		
		await _dbContext.Users.AddAsync(userEntity);
		await _dbContext.SaveChangesAsync();

		return userEntity.Id;

	}

	public async Task<bool> IsEmailTaken(string email)
	{
		bool isEmailTaken = await _dbContext.Users
			.AnyAsync(u => string.Equals(u.Email, email));
		
		return isEmailTaken;
		
	}
	
	public async Task<User> GetByEmail(string email)
	{

		var userEntity = await _dbContext.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => string.Equals(u.Email, email));
		
		if (userEntity == null)
			throw new KeyNotFoundException($"User with email {email} not found");

		var user = new User(userEntity.Id, userEntity.Email,
			userEntity.Username, userEntity.HashedPassword, userEntity.IsAdmin);
		
		
		return user;
		
	}

	public async Task<User> UpdateIsAdmin(User user)
	{
		
		await _dbContext.Users
			.Where(r => r.Id == user.Id)
			.ExecuteUpdateAsync(e => e
				.SetProperty(p => p.IsAdmin, p => user.IsAdmin));
		
		return user;
		
	}
	
	
	
}