namespace FilmRatings.Contracts;

public record PageResponse<T>(
	int CurrentPage,
	int TotalPages,
	int TotalCount,
	IEnumerable<T> Items
	);