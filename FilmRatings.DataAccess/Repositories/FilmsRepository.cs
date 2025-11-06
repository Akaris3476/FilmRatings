using FilmRatings.Core.Abstractions;
using FilmRatings.Core.Models;
using FilmRatings.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmRatings.DataAccess.Repositories;

public class FilmsRepository : IFilmsRepository
{
	private readonly FilmRatingsDbContext _dbContext;

	public FilmsRepository(FilmRatingsDbContext dbContext)
	{
		_dbContext = dbContext;
	}


	public async Task<List<Film>> GetAll()
	{
		List<FilmEntity> filmEntities = await _dbContext.Films
			.AsNoTracking()
			.ToListAsync();
		
		List<Film> films =  filmEntities
			.Select(filmEntity => new Film(filmEntity.Id, filmEntity.Title, filmEntity.Description))
			.ToList();
		
		return films;
	}
	
	public async Task<Film> GetById(Guid id)
	{
		var filmEntity = await _dbContext.Films
			.AsNoTracking()
			.FirstOrDefaultAsync(film => film.Id == id);
		
		if (filmEntity == null)
			throw new KeyNotFoundException($"Film {id} not found");

		Film film = new(filmEntity.Id, filmEntity.Title, filmEntity.Description);



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
		
		return film.Id;
	}
	
	public async Task<Guid> Update(Guid id, string title, string description)
	{
	
		await _dbContext.Films
			.Where(f => f.Id == id)
			.ExecuteUpdateAsync(e => e
				.SetProperty(p => p.Title, p => title)
				.SetProperty(p => p.Description, p => description));
	
		return id;
	}
	
	public async Task<Guid> Delete(Guid id)
	{
		await _dbContext.Films
			.Where(f => f.Id == id)
			.ExecuteDeleteAsync();
		
		return id;
	}
	
	
	
}