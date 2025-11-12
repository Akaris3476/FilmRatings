namespace FilmRatings.Contracts;

public record RatingsResponse(
	Guid Id,
	// string Title,
	// string username,
	int Rating
	);