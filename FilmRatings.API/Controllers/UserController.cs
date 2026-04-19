using FilmRatings.Contracts.Users;
using FilmRatings.Core.Abstractions.Services;
using FilmRatings.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmRatings.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
	private readonly IUsersService _usersService;

	public UserController(IUsersService usersService)
	{
		_usersService = usersService;
	}

	[Authorize(Policy="AdminPolicy")]
	[HttpPatch("{email}")]
	public async Task<ActionResult<Guid>> ChangeAdminRights(string email, [FromBody] UserRequest request )
	{
		User user = await _usersService.ChangeAdminRights(email, request.isAdmin);
		return Ok(user.Email);
	}
	
}