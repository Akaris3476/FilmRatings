using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace FilmRatings.Infrastructure.Entities;

[Index(nameof(Token), IsUnique = true)]
public class RefreshTokenEntity
{
	[Key]
	public Guid Id { get; set; }
	
	[MaxLength(200)]
	public string Token { get; set; } = string.Empty;
	
	public Guid UserId { get; set; }
	public UserEntity User { get; set; }  = null!;
	public DateTime ExpiresOnUTC { get; set; }

	
}