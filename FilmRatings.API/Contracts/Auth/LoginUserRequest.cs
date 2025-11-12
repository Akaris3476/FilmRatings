using System.ComponentModel.DataAnnotations;

namespace FilmRatings.Contracts;

public record LoginUserRequest(
	[Required] string Email,
	[Required] string Password
	);