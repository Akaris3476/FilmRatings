using FilmRatings.Application.Services;
using FilmRatings.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FilmRatings.Controllers;

[ApiController]
[Route("auth")]
public class UsersController  : ControllerBase
{
	private readonly IUsersService _usersService;

	public UsersController(IUsersService usersService)
	{
		_usersService = usersService;
	}
	
	[HttpPost("register")]
	public async Task<ActionResult> Register([FromBody]RegisterUserRequest request)
	{
		
		try
		{
			await _usersService.Register(request.username, request.email, request.password);
		
			return Ok();
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}

	}

	[HttpPost("login")]
	public async Task<ActionResult<string>> Login([FromBody]LoginUserRequest request)
	{

		try
		{
			var token = await _usersService.Login(request.Email, request.Password);
			
			HttpContext.Response.Cookies.Append("pechenka", token);
			
			return Ok(token);
		}
		catch (Exception e)
		{
			return BadRequest(e.Message);
		}
		
		
	}
	
	
	
}