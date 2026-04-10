namespace FilmRatings.Contracts.Ratings;

public record RatingsResponse(
	Guid Id,
	// string Title,
	// string username,
	int Rating
	);