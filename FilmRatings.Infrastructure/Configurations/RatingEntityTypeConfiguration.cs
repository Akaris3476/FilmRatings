using FilmRatings.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilmRatings.Infrastructure.Configurations;


// public class RatingEntityTypeConfiguration : IEntityTypeConfiguration<RatingEntity>
// {
// 	public void Configure(EntityTypeBuilder<RatingEntity> builder)
// 	{
// 		builder
// 			.Property(b => b.UserId)
// 			.IsRequired(false);
// 	}
// }