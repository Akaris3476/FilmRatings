using System.ComponentModel.DataAnnotations;

namespace FilmRatings.Contracts.Auth;

public record LoginUserRequest(
	[Required] string Email,
	[Required] string Password
	);