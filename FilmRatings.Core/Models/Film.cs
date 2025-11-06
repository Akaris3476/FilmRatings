namespace FilmRatings.Core.Models;

public class Film
{
	
	private List<Rating> _ratings = new();
	
	public Guid Id { get; private set; }
	public string Title { get; private set; } = string.Empty;
	
	public string Description { get; private set; } = string.Empty;
	
	public IReadOnlyCollection<Rating> Ratings => _ratings.AsReadOnly();

	public Film(Guid id, string title, string description)
	{
		Id = id;
		SetTitle(title);
		SetDescription(description);
	}
	

	
	public void SetTitle(string title)
	{
		if (string.IsNullOrWhiteSpace(title))
			throw new ArgumentException("Title cannot be null or whitespace", nameof(title));
		
		if (title.Length > 75)
			throw new ArgumentException("Title cannot be longer than 75 characters", nameof(title));
		
		Title = title;
	}

	public void SetDescription(string description)
	{
		if (string.IsNullOrWhiteSpace(description))
			throw new ArgumentException("Description cannot be null or whitespace", nameof(description));
		
		if (description.Length > 2000)
			throw new ArgumentException("Description cannot be longer than 2000 characters", nameof(description));
		
		Description = description;
	}
	
	public void AddRating(Rating rating)
	{
		if (_ratings.Exists(r => r.Id == rating.Id))
			throw new ArgumentException("Rating already exists");
			
		_ratings.Add(rating);
	}

	public void SetRatingList(List<Rating> ratings)
	{
		_ratings = ratings;
	}
	
	
	public void RemoveRating(Rating rating)
	{
		_ratings.Remove(rating);
	}

	public float AverageRating()
	{
		if (_ratings.Count == 0)
			return 0;

		var averageRating = _ratings.Average(r => r.Value);
		
		return (float)Math.Round(averageRating,2);
	}
	
	public static float AverageRating(List<Rating> ratings)
	{
		if (ratings.Count == 0)
			return 0;
		
		var averageRating = ratings.Average(r => r.Value);
		
		return (float)Math.Round(averageRating,2);
	}



}