using FilmRatings.Core.Models;

namespace FilmRatings.Core.Abstractions.Repositories;

public interface IUsersRepository
{
	Task<Guid> Add(User user);
	Task<User> GetByEmail(string email);
	Task<bool> IsEmailTaken(string email);
	Task<User> UpdateIsAdmin(User user);
}