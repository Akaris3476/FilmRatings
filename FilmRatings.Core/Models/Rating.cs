namespace FilmRatings.Core.Models;

public class Rating
{
	public Guid Id { get; private set; }
	public int Value { get; private set; }
	public Film Film { get; private set; }
	public Guid FilmId { get; private set; }
	
	// public User User { get; private set; }
	//
	// public Guid UserId { get; private set; }
	
	// TODO: add user owner

	public Rating(Guid id, int value, Film film)
	{
		Id = id;
		SetValue(value);
		FilmId = film.Id;
		Film = film;
		
	}
	
	public void SetValue(int value)
	{
		if (value < 1 || value > 10)
			throw new ArgumentException("Rating must be between 0 and 10");
		
		Value = value;
	}
}