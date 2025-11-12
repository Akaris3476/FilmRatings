using FilmRatings.Core.Abstractions;
using FilmRatings.Core.Abstractions.Auth;
using FilmRatings.Core.Models;

namespace FilmRatings.Application.Services;

public class UsersService : IUsersService
{
	private readonly IJwtProvider _jwtProvider;
	private readonly IUsersRepository _usersRepository;
	private readonly IPasswordHasher _passwordHasher;

	public UsersService(
		IJwtProvider jwtProvider,
		IUsersRepository usersRepository,
		IPasswordHasher passwordHasher)
	{
		_jwtProvider = jwtProvider;
		_usersRepository = usersRepository;
		_passwordHasher = passwordHasher;
	}
	
	public async Task Register(string username, string email, string password)
	{

		if (password.Length < 6)
			throw new ArgumentException("Password must be at least 6 characters long");
		
		var hashedPassword = _passwordHasher.Generate(password);

		var user = new User(Guid.NewGuid(), email, username, hashedPassword);

		await _usersRepository.Add(user);
		
	}

	public async Task<string> Login(string email, string password)
	{
		var user = await _usersRepository.GetByEmail(email);
		
		var result = _passwordHasher.Verify(password, user.HashedPassword);
		
		if (!result)
			throw new ArgumentException("Invalid password");
		
		var token = _jwtProvider.GenerateToken(user.Id);
		
		return token;
	}
}