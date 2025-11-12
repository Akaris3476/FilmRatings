
namespace FilmRatings.Infrastructure.Entities;

public class RatingEntity
{
	public Guid Id { get; set; }
	
	public int Value { get; set; }
	
	public Guid? UserId { get; set; }
	
	public UserEntity User { get; set; } = null!; 
	
	public Guid FilmId { get; set; }

	public FilmEntity Film { get; set; } = null!;
}