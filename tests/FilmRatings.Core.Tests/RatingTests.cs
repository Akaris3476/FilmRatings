using FilmRatings.Core.Models;
using FluentAssertions;

namespace FilmRatings.Core.Tests;

public class RatingTests
{
	[Theory]
	[InlineData(0)]
	[InlineData(11)]
	[InlineData(null)]
	public void SetValue_ShouldBeFrom1To10(int value)
	{

		var film = new Film(Guid.Empty, "superman", "some title");
		

		Assert.Throws<ArgumentException>(() => new Rating(Guid.NewGuid(), value, film));
	}
}