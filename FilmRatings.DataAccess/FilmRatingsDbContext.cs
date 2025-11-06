using FilmRatings.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmRatings.DataAccess;

public class FilmRatingsDbContext : DbContext
{
	
	public FilmRatingsDbContext(DbContextOptions<FilmRatingsDbContext> options) : base(options)
	{
		
	}
	
	public DbSet<FilmEntity> Films { get; set; }
	public DbSet<RatingEntity> Ratings { get; set; }
	
}