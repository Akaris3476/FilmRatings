using System.Text;
using FilmRatings.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FilmRatings.Extensions;

public static class ApiExtensions
{

	public static void AddApiAuthentication(
		this IServiceCollection services, IConfiguration configuration)
	{
		
		var jwtOptions = configuration
			.GetSection(nameof(JwtOptions))
			.Get<JwtOptions>();

		
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = false,
					ValidateAudience = false,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions!.SecretKey))

				};

				options.Events = new JwtBearerEvents()
				{
					OnMessageReceived = context =>
					{
						context.Token = context.Request.Cookies["pechenka"];

						return Task.CompletedTask;
					}
				};

			});

		services.AddAuthorization(options =>
		{
			options.AddPolicy("AdminPolicy", policy =>
			{
				policy.RequireClaim("Admin", "True");
			});
			
		});
	}
		
}