using FilmRatings.Core.Models;

namespace FilmRatings.Core.Abstractions.Services;

public interface IUsersService
{
	Task Register(string username, string email, string password);
	Task<(string, string)> Login(string email, string password);
	Task<User> ChangeAdminRights(string email, bool isAdmin);
}