using FilmRatings.Core.Abstractions;
using FilmRatings.Core.Models;
using FilmRatings.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace FilmRatings.Infrastructure.Repositories;

public class FilmsRepository : IFilmsRepository
{
	private readonly FilmRatingsDbContext _dbContext;
	private readonly IDistributedCache _distributedCache;

	public FilmsRepository(FilmRatingsDbContext dbContext, IDistributedCache distributedCache)
	{
		_dbContext = dbContext;
		_distributedCache = distributedCache;
	}


	public async Task<List<Film>> GetAll()
	{
				
		string key = "all-films";
		
		string? cachedFilm = await _distributedCache.GetStringAsync(key);

		if (!string.IsNullOrEmpty(cachedFilm))
		{
			var cacheFilms = JsonConvert.DeserializeObject<List<Film>>(cachedFilm);
			
			if (cacheFilms is not null) 
				return cacheFilms;
		}


		List<FilmEntity> filmEntities = await _dbContext.Films
			.AsNoTracking()
			.ToListAsync();
		
		List<Film> films =  filmEntities
			.Select(filmEntity => new Film(filmEntity.Id, filmEntity.Title, filmEntity.Description))
			.ToList();
		
		await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(films));

		return films;
	}
	
	public async Task<Film> GetById(Guid id)
	{
		
		string key = $"{nameof(Film)}-{id}";
		
		string? cachedFilm = await _distributedCache.GetStringAsync(key);

		if (!string.IsNullOrEmpty(cachedFilm))
		{
			var cacheFilm = JsonConvert.DeserializeObject<Film>(cachedFilm);
			
			 if (cacheFilm is not null) 
				 return cacheFilm;
		}
		
		
		var filmEntity = await _dbContext.Films
			.AsNoTracking()
			.FirstOrDefaultAsync(film => film.Id == id);
		
		
		if (filmEntity == null)
			throw new KeyNotFoundException($"Film {id} not found");
		
		
		Film film = new(filmEntity.Id, filmEntity.Title, filmEntity.Description);
		
		await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(film));
		
		return film;

	}

	public async Task<Guid> Create(Film film)
	{

		var filmEntity = new FilmEntity
		{
			Id = film.Id,
			Title = film.Title,
			Description = film.Description
		};
	
		await _dbContext.AddAsync(filmEntity);
		await _dbContext.SaveChangesAsync();
		
		_distributedCache.Remove("all-films");
		
		return film.Id;
	}
	
	public async Task<Guid> Update(Guid id, string title, string description)
	{
	
		await _dbContext.Films
			.Where(f => f.Id == id)
			.ExecuteUpdateAsync(e => e
				.SetProperty(p => p.Title, p => title)
				.SetProperty(p => p.Description, p => description));
	
		_distributedCache.Remove($"{nameof(Film)}-{id}");
		_distributedCache.Remove("all-films");

		return id;
	}
	
	public async Task<Guid> Delete(Guid id)
	{
		await _dbContext.Films
			.Where(f => f.Id == id)
			.ExecuteDeleteAsync();
		
		_distributedCache.Remove($"{nameof(Film)}-{id}");
		_distributedCache.Remove("all-films");

		return id;
	}
	
	
	
}