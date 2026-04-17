
using System.ComponentModel.DataAnnotations;

namespace FilmRatings.Infrastructure.Entities;

public class RatingEntity
{
	public Guid Id { get; set; }
	
	public int Value { get; set; }
	
	public Guid? UserId { get; set; }
	
	[MaxLength(20)]
	public string? Username { get; set; }
	
	public UserEntity? User { get; set; } 
	
	public Guid FilmId { get; set; }

	public FilmEntity Film { get; set; } = null!;
}