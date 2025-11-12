using FilmRatings.Core.Models;

namespace FilmRatings.Core.Abstractions;

public interface IUsersRepository
{
	Task<Guid> Add(User user);
	Task<User> GetByEmail(string email);

}