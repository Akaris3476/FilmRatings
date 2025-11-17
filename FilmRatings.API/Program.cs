using FilmRatings.Application.Services;
using FilmRatings.Core.Abstractions;
using FilmRatings.Core.Abstractions.Auth;
using FilmRatings.Exceptions;
using FilmRatings.Extensions;
using FilmRatings.Infrastructure;
using FilmRatings.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<FilmRatingsDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(FilmRatingsDbContext)))
);


builder.Services.Configure<JwtOptions>(
	builder.Configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = builder.Configuration.GetConnectionString("Redis");
	options.InstanceName = "FilmsCache";
});


builder.Services.AddScoped<IFilmsRepository, FilmsRepository>();
builder.Services.AddTransient<IFilmsService, FilmsService>();

builder.Services.AddScoped<IRatingsRepository, RatingsRepository>();
builder.Services.AddTransient<IRatingsService, RatingsService>();

builder.Services.AddScoped<IUsersRepository,UsersRepository>();
builder.Services.AddTransient<IUsersService, UsersService>();

builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();


builder.Services.AddApiAuthentication(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandler>();

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();

