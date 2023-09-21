using ApiPeliculas.Data;
using ApiPeliculas.Shared;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Feats.Movies.Repository;

public class MovieRepository : IMovieRepository
{
    private readonly AppDbContext context;

    public MovieRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<Movie?> GetMovieById(Guid id)
    {
        return await context.Movies
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cat => cat.Id == id);
    }

    public async Task<ICollection<Movie>> GetMovies()
    {
        return await context.Movies
            .AsNoTracking()
            .OrderBy(cat => cat.Name)
            .ToListAsync();
    }

    public async Task<bool> CreateMovie(Movie Movie)
    {
        Movie.CreationDate = DateTimeOffset.UtcNow;
        await context.AddAsync(Movie);
        return await Save();
    }

    public async Task<bool> UpdateMovie(Movie Movie)
    {
        Movie? MovieBd = await context.Movies.FindAsync(Movie.Id);
        if (MovieBd is null)
        {
            return false;
        }

        Movie.CreationDate = DateTimeOffset.UtcNow;
        context.Entry(MovieBd).CurrentValues.SetValues(Movie);

        return await Save();
    }

    public async Task<bool> DeleteMovie(Movie Movie)
    {
        context.Movies.Remove(Movie);
        return await Save();
    }

    public bool MovieExists(string name)
    {
        return context.Movies.Any(cat =>
                    cat.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool MovieExists(Guid id)
    {
        return context.Movies.Any(cat =>
                    cat.Id == id);
    }

    public async Task<ICollection<Movie>> GetMoviesInCategories(Guid CategoryId)
    {
        return await context.Movies.Include(ca => ca.Category)
            .Where(ca => ca.CategoryId == CategoryId)
            .ToListAsync();
    }

    public async Task<ICollection<Movie>> Search(string name)
    {
        IQueryable<Movie> movies = context.Movies;
        if (!String.IsNullOrEmpty(name))
        {
            movies = movies
                .Where(movie => movie.Name.Contains(name) || movie.Description.Contains(name));
        }

        return await movies.ToListAsync();
    }

    public async Task<bool> Save()
    {
        return await context.SaveChangesAsync() != 0;
    }
}
