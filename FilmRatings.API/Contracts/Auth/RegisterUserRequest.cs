using System.ComponentModel.DataAnnotations;

namespace FilmRatings.Contracts;

public record RegisterUserRequest(
	[Required] string username,
	[Required] string email,
	[Required] string password
	);