
namespace FilmRatings.DataAccess.Entities;

public class RatingEntity
{
	public Guid Id { get; set; }
	
	public int Value { get; set; }
	
	public Guid FilmId { get; set; }

	public FilmEntity Film { get; set; } = null!;
}