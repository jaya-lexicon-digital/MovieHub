using MovieHub.API.Entities;

namespace MovieHub.API.Services;

public interface IMovieHubRepository
{
    Task<(IEnumerable<Movie>, PaginationMetadata)> GetMoviesAsync(string? title, string? genre, 
        int pageNumber, int pageSize);
    
    Task<Movie?> GetMovieAsync(int movieId, bool includeCinemas);
}