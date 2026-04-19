using Newtonsoft.Json;

namespace FilmRatings.Core.Models;

public class Rating
{
	public Guid Id { get; private set; }
	public int Value { get; private set; }
	public Guid FilmId { get; private set; }
	
	public Guid? UserId { get; private set; }
	public string? Username { get; private set; }
	

	public Rating(Guid id, int value, Guid filmId)
	{
		Id = id;
		SetValue(value);
		FilmId = filmId;
		
	}
	
	[JsonConstructor]
	public Rating(Guid id, int value, Guid filmId, Guid? userId,  string? username)
	{
		Id = id;
		SetValue(value);
		FilmId = filmId;
		
		SetUser(userId, username);
		
	}

	public void SetUser(Guid? userId, string? username)
	{
		UserId = userId;
		Username = username;
		
	}
	public void SetValue(int value)
	{
		if (value < 1 || value > 10)
			throw new ArgumentException("Rating must be between 0 and 10");
		
		Value = value;
	}
}