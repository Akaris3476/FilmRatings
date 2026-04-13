namespace FilmRatings.Contracts.Ratings;

public record RatingsResponse(
	Guid Id,
	Guid? UserId,
	string? Username,
	int Rating
	);