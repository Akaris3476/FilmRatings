using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace FilmRatings.Infrastructure.Entities;

public class FilmEntity
{
	[Key]
	public Guid Id { get; set; }
	
	[MaxLength(100)]
	[Required]
	public string Title { get; set; } = string.Empty;
	
	[MaxLength(2000)]
	public string Description { get; set; } = string.Empty;

	public ICollection<RatingEntity> Ratings { get; set; } = new List<RatingEntity>();
}