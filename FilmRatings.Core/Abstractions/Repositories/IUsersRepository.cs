using FilmRatings.Core.Models;

namespace FilmRatings.Core.Abstractions.Repositories;

public interface IUsersRepository
{
	Task<Guid> Add(User user);
	Task<User> GetByEmail(string email);

}