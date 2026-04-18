using FilmRatings.Core.Abstractions.Repositories;
using FilmRatings.Core.Models;
using FilmRatings.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmRatings.Infrastructure.Repositories;

public class FilmsRepository : IFilmsRepository
{
	private readonly FilmRatingsDbContext _dbContext;
	private readonly ICacheService _cacheService;
	
	private readonly TimeSpan _filmCacheDuration = TimeSpan.FromMinutes(2);
	private readonly TimeSpan _allFilmsCacheDuration = TimeSpan.FromMinutes(4);
	private readonly TimeSpan _allFilmsWithTitleCacheDuration = TimeSpan.FromMinutes(1);
	private readonly TimeSpan _filmsCountDuration = TimeSpan.FromMinutes(5);

	public int pageSize => 5;

	public FilmsRepository(FilmRatingsDbContext dbContext,  ICacheService cacheService)
	{
		_dbContext = dbContext;
		_cacheService = cacheService;
	}
	
	
	
	public async Task<List<Film>> GetAll(
		string? title, int page, FilmsIncludeOptions includeOptions)
	{
		string key = $"all-films-include-{(int)includeOptions}-title-{title}-page-{page}";

		List<Film>? cachedFilms = await _cacheService.GetAsync<List<Film>>(key);
		
		if (cachedFilms is not null)
				return cachedFilms;
		
		int filmsToSkip = (page - 1) * pageSize;
		
		IQueryable<FilmEntity> filmsQuery= _dbContext
			.Films
			.OrderBy(f => f.Id)
			.Skip(filmsToSkip)
			.Take(pageSize)
			.AsNoTracking();

		if (title is not null)
			filmsQuery = filmsQuery.Where(f => f.Title.ToLower().Contains(title.ToLower()));
		
		filmsQuery = ApplyIncludeOptionsToQuery(filmsQuery, includeOptions);
		


		List<FilmEntity> filmEntities = await filmsQuery.ToListAsync();
		
		var rawList = filmEntities
			.Select(f => new 
			{
				f.Id,
				f.Title,
				f.Description,
				Ratings = f.Ratings.Select(r => new { r.Id, r.Value, r.UserId, r.Username }).ToList()
			});
		
		
		List<Film> films =  rawList
			.Select(entity =>
			{
				var film = new Film(entity.Id, entity.Title, entity.Description);

				var ratings = entity.Ratings
					.Select(r => new Rating(r.Id, r.Value, film.Id, r.UserId, r.Username));
				
				film.SetRatingList(ratings);

				return film;

			})
			.ToList();

		if (title is not null) {
			await _cacheService.SetAsync(key, films, _allFilmsWithTitleCacheDuration);
		}
		else {
			await _cacheService.SetAsync(key, films, _allFilmsCacheDuration);
		}

		return films;
	}
	
	public async Task<Film> GetById(Guid id, FilmsIncludeOptions includeOptions)
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

	public async Task<int> GetFilmCount()
	{
		string key = $"{nameof(Film)}-{nameof(GetFilmCount)}";
		int? cachedCount = await _cacheService.GetAsync<int?>(key);
		
		if (cachedCount is not null)
			return cachedCount.Value;
		
		int count = await _dbContext.Films.CountAsync();
		
		await _cacheService.SetAsync(key, count, _filmsCountDuration);
		
		return count;
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
		await _cacheService.RemoveAsync($"{nameof(Film)}-{nameof(GetFilmCount)}");
		
		int filmCount = await GetFilmCount();
		int pageCount = (int)Math.Ceiling((double)filmCount / pageSize);
		foreach (var includeOption in Enum.GetValues<FilmsIncludeOptions>())
		{
			for (int i = 0; i < pageCount; i++)
			{
				await _cacheService.RemoveAsync($"all-films-include-{(int)includeOption}-title--page-{pageCount}");
			}
			await _cacheService.RemoveAsync($"{nameof(Film)}-{id}-include-{(int)includeOption}");
		}
	}
	
	
	
}