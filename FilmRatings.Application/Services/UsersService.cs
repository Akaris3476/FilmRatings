using System.Text.RegularExpressions;
using FilmRatings.Core.Abstractions.Auth;
using FilmRatings.Core.Abstractions.Repositories;
using FilmRatings.Core.Abstractions.Services;
using FilmRatings.Core.Models;

namespace FilmRatings.Application.Services;

public class UsersService : IUsersService
{
	private readonly IJwtProvider _jwtProvider;
	private readonly IUsersRepository _usersRepository;
	private readonly IPasswordHasher _passwordHasher;
	private readonly IAuthService _authService;

	public UsersService(
		IJwtProvider jwtProvider,
		IUsersRepository usersRepository,
		IPasswordHasher passwordHasher,
		IAuthService authService)
	{
		_jwtProvider = jwtProvider;
		_usersRepository = usersRepository;
		_passwordHasher = passwordHasher;
		_authService = authService;
	}
	
	public async Task Register(string username, string email, string password)
	{
		string pattern = @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b";
		Regex regex = new(pattern, RegexOptions.IgnoreCase);
		if (!regex.IsMatch(email))
			    throw new ArgumentException("Please, provide a valid email address");
		
		if (password.Length < 6)
			throw new ArgumentException("Password must be at least 6 characters long");
		
		bool isEmailTaken = await _usersRepository.IsEmailTaken(email);
		if (isEmailTaken)
			throw new ArgumentException("Email is already taken. Try another email");
		
		
		var hashedPassword = _passwordHasher.Generate(password);

		var user = new User(Guid.NewGuid(), email, username, hashedPassword);

		await _usersRepository.Add(user);
		
	}

	public async Task<(string, string)> Login(string email, string password)
	{
		User user = await _usersRepository.GetByEmail(email);
		
		bool result = _passwordHasher.Verify(password, user.HashedPassword);
		
		if (!result)
			throw new ArgumentException("Invalid password");
		
		string token = _jwtProvider.GenerateToken(user.Id, user.Username, user.IsAdmin);
		
		string refreshToken = _jwtProvider.GenerateRefreshTokenString();
		await _authService.SaveRefreshToken(refreshToken, user.Id);
		
		return (token, refreshToken);
	}

	public async Task<User> ChangeAdminRights(string email, bool isAdmin)
	{
		User user = await _usersRepository.GetByEmail(email);
		user.IsAdmin = isAdmin;
		
		return await _usersRepository.UpdateIsAdmin(user);
		
	}
}