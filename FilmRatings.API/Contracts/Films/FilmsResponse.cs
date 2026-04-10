using FilmRatings.Contracts.Ratings;

namespace FilmRatings.Contracts.Films;

public record FilmsResponse(
	Guid Id,
	string Title,
	string Description,
	float? AverageRating = null,
	IReadOnlyCollection<RatingsResponse>? Ratings = null
);
