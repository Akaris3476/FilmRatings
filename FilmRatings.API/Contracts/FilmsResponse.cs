using FilmRatings.Core.Models;

namespace FilmRatings.Contracts;

public record FilmsResponse(
	Guid Id,
	string Title,
	string Description
	// float AverageRating
	// IReadOnlyCollection<RatingsResponse> Ratings
);

public record FilmWithRatingsResponse(
	Guid Id,
	string Title,
	string Description,
	float AverageRating,
	IReadOnlyCollection<RatingsResponse> Ratings
	);