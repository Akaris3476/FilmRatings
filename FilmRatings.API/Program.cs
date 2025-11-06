using FilmRatings.Application.Services;
using FilmRatings.Core.Abstractions;
using FilmRatings.DataAccess;
using FilmRatings.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<FilmRatingsDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(FilmRatingsDbContext)))
);

builder.Services.AddTransient<IFilmsRepository, FilmsRepository>();
builder.Services.AddTransient<IFilmsService, FilmsService>();

builder.Services.AddTransient<IRatingsRepository, RatingsRepository>();
builder.Services.AddTransient<IRatingsService, RatingsService>();

// FilmRatingsDbContext dbContext = builder.Services.BuildServiceProvider().GetRequiredService<FilmRatingsDbContext>();
// if (!dbContext.Database.CanConnect()){
// 	Console.WriteLine("--- no db :( ---");
// }

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapControllers();

app.Run();

