using FilmRatings.Contracts.Auth;
using FilmRatings.Core.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace FilmRatings.Controllers;

[ApiController]
[Route("auth")]
public class AuthController  : ControllerBase
{
	private readonly IUsersService _usersService;

	public AuthController(IUsersService usersService)
	{
		_usersService = usersService;
	}
	
	
	[HttpPost("register")]
	public async Task<ActionResult> Register([FromBody]RegisterUserRequest request)
	{
		
		await _usersService.Register(request.username, request.email, request.password);
		
		return Ok();
			
	}

	
	[HttpPost("login")]
	public async Task<ActionResult<string>> Login([FromBody]LoginUserRequest request)
	{

		var token = await _usersService.Login(request.Email, request.Password);
			
		HttpContext.Response.Cookies.Append("pechenka", token); 
			
		return Ok(token);
		
	}
	
		
		
}