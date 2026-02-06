using FilmRatings.Core.Models;
using FluentAssertions;

namespace FilmRatings.Core.Tests;

public class FilmTests
{
	[Fact]
	public void AverageRating_WhenRatingsExist_ShouldGiveCorrectValue()
	{
		
		var film = new Film(Guid.Empty, "Superman", "some description");

		
		var ratingList = new List<Rating>
		{
			new(Guid.Empty, 7, film),
			new(Guid.Empty, 9, film),
			new(Guid.Empty, 10, film),
			new(Guid.Empty, 4, film),
			new(Guid.Empty, 2, film),
			new(Guid.Empty, 5, film),
		};
		film.SetRatingList(ratingList);
		
		
		film.AverageRating().Should().Be(6.17F);

	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public void SetDescription_BlankDescription_ShouldThrowArgumentException(string? description)
	{
		var film = new Film(Guid.Empty, "Superman", "some description");
		
		Assert.Throws<ArgumentException>(() => film.SetDescription(description));
		
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public void SetTitle_BlankTitle_ShouldThrowArgumentException(string? title)
	{
		var film = new Film(Guid.Empty, "Superman", "some description");

		Assert.Throws<ArgumentException>(() => film.SetTitle(title));
	}

	[Fact]
	public void AddRating_ShouldAddRating()
	{
		var film = new Film(Guid.Empty, "Superman", "some description");
		
		var guid = Guid.Empty;
		var rating = new Rating(guid, 5, film);
		
		film.AddRating(rating);
		
		film.Ratings.Should().Contain(rating);

	}

	[Fact]
	public void StaticAverageRating_ShouldEqualNonStatic()
	{
		var film = new Film(Guid.Empty, "Superman", "some description");
		
		var ratingList = new List<Rating>
		{
			new(Guid.Empty, 7, film),
			new(Guid.Empty, 9, film),
			new(Guid.Empty, 10, film),
			new(Guid.Empty, 4, film),
			new(Guid.Empty, 2, film),
			new(Guid.Empty, 5, film),
		};
		film.SetRatingList(ratingList);

		film.AverageRating().Should().Be(Film.AverageRating(ratingList));
	}


}