using FilmRatings.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmRatings.Infrastructure;

public class FilmRatingsDbContext : DbContext
{
	
	public FilmRatingsDbContext(DbContextOptions<FilmRatingsDbContext> options) : base(options)
	{
		
	}
	
	public DbSet<FilmEntity> Films { get; set; }
	public DbSet<RatingEntity> Ratings { get; set; }
	
	public DbSet<UserEntity> Users { get; set; }
	
}