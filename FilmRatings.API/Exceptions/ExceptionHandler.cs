using Microsoft.AspNetCore.Mvc;

namespace FilmRatings.Exceptions;

internal sealed class ExceptionHandler(
	RequestDelegate next, ILogger<ExceptionHandler> logger)
{
	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await next(context);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Unhandled exception");

			context.Response.StatusCode = e switch
			{
				ApplicationException => StatusCodes.Status400BadRequest,
				ArgumentException => StatusCodes.Status400BadRequest,
				KeyNotFoundException => StatusCodes.Status404NotFound,
				_ => StatusCodes.Status500InternalServerError
			};
			
			
			await context.Response.WriteAsJsonAsync(
				new ProblemDetails
				{
					Type = e.GetType().Name,
					Title = "An error occured",
					Detail = e.Message
				});
		}
	}
}