using FilmRatings.Core.Abstractions.Repositories;
using FilmRatings.Core.Models;
using FilmRatings.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmRatings.Infrastructure.Repositories;

public class FilmsRepository : IFilmsRepository
{
	private readonly FilmRatingsDbContext _dbContext;
	private readonly ICacheService _cacheService;
	
	private readonly TimeSpan _filmCacheDuration = TimeSpan.FromMinutes(5);
	private readonly TimeSpan _allFilmsCacheDuration = TimeSpan.FromHours(10);

	public FilmsRepository(FilmRatingsDbContext dbContext,  ICacheService cacheService)
	{
		_dbContext = dbContext;
		_cacheService = cacheService;
	}
	
	
	
	public async Task<List<Film>> GetAll(FilmsIncludeOptions includeOptions = FilmsIncludeOptions.None) 
	{
		string key = $"all-films-include-{(int)includeOptions}";
		
		List<Film>? cachedFilms = await _cacheService.GetAsync<List<Film>>(key);
		
		if (cachedFilms is not null)
				return cachedFilms;
		
		
		IQueryable<FilmEntity> filmsQuery= _dbContext
			.Films
			.AsNoTracking();
		
		filmsQuery = ApplyIncludeOptionsToQuery(filmsQuery, includeOptions);
		


		List<FilmEntity> filmEntities = await filmsQuery.ToListAsync();
		
		var rawList = filmEntities
			.Select(f => new 
			{
				f.Id,
				f.Title,
				f.Description,
				Ratings = f.Ratings.Select(r => new { r.Id, r.Value }).ToList()
			});
		
		
		List<Film> films =  rawList
			.Select(entity =>
			{
				var film = new Film(entity.Id, entity.Title, entity.Description);

				var ratings = entity.Ratings
					.Select(r => new Rating(r.Id, r.Value, film.Id));
				
				film.SetRatingList(ratings);

				return film;

			})
			.ToList();
		
		await _cacheService.SetAsync(key, films, _allFilmsCacheDuration);

		return films;
	}
	
	public async Task<Film> GetById(Guid id, FilmsIncludeOptions includeOptions = FilmsIncludeOptions.None)
	{
		
		string key = $"{nameof(Film)}-{id}-include-{(int)includeOptions}";
		
		Film? cachedFilm = await _cacheService.GetAsync<Film>(key);
		
		if (cachedFilm is not null) 
			return cachedFilm;


		IQueryable<FilmEntity> filmQuery = _dbContext.Films
			.AsNoTracking();
			
		
		filmQuery = ApplyIncludeOptionsToQuery(filmQuery, includeOptions);
		
		FilmEntity? filmEntity = await filmQuery.FirstOrDefaultAsync(film => film.Id == id);
		
		if (filmEntity == null)
			throw new KeyNotFoundException($"Film {id} not found");
		
		
		Film film = new(filmEntity.Id, filmEntity.Title, filmEntity.Description);
		film.SetRatingList(filmEntity.Ratings
			.Select(r => new Rating(r.Id, r.Value, r.FilmId, r.UserId, r.User?.Username)));
		
		await _cacheService.SetAsync(key, film, _filmCacheDuration);
		
		return film;

	}
	
	private IQueryable<FilmEntity> ApplyIncludeOptionsToQuery(IQueryable<FilmEntity> query, FilmsIncludeOptions includeOptions)
	{
		FilmsIncludeOptions[] possibleOptions = Enum.GetValues<FilmsIncludeOptions>();
		
		foreach (var option in possibleOptions)
		{
			if (option == FilmsIncludeOptions.None || !includeOptions.HasFlag(option)) 
				continue;
			
			query = option switch
			{
				FilmsIncludeOptions.Ratings => query.Include(f => f.Ratings),
				_ => query
			};
		}
		return query;
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
		
		await InvalidateCache();
		
		return film.Id;
	}
	
	public async Task<Guid> Update(Guid id, string title, string description)
	{
	
		await _dbContext.Films
			.Where(f => f.Id == id)
			.ExecuteUpdateAsync(e => e
				.SetProperty(p => p.Title, p => title)
				.SetProperty(p => p.Description, p => description));
	
		await InvalidateCache(id);

		return id;
	}
	
	public async Task<Guid> Delete(Guid id)
	{
		await _dbContext.Films
			.Where(f => f.Id == id)
			.ExecuteDeleteAsync();

		await InvalidateCache(id);

		return id;
	}
	
	private async Task InvalidateCache(Guid? id = null)
	{
		if (id is not null)
			await _cacheService.RemoveAsync($"{nameof(Film)}-{id}");
		
		await _cacheService.RemoveAsync("all-films");
		foreach (var includeOption in Enum.GetValues<FilmsIncludeOptions>())
		{
			await _cacheService.RemoveAsync($"all-films-include-{(int)includeOption}");
			await _cacheService.RemoveAsync($"{nameof(Film)}-{id}-include-{(int)includeOption}");
		}
	}
	
	
	
}