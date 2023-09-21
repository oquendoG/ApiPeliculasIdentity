using ApiPeliculas.Shared;

namespace ApiPeliculas.Feats.Movies.Repository;

public interface IMovieRepository
{
    bool MovieExists(string name);
    bool MovieExists(Guid id);
    Task<bool> CreateMovie(Movie movie);
    Task<bool> DeleteMovie(Movie movie);
    Task<ICollection<Movie>> GetMovies();
    Task<Movie?> GetMovieById(Guid id);
    Task<bool> Save();
    Task<bool> UpdateMovie(Movie movie);
    Task<ICollection<Movie>> GetMoviesInCategories(Guid CategoryId);
    Task<ICollection<Movie>> Search(string name);
}
