using MovieHub.API.Entities;

namespace MovieHub.API.Services;

public interface IMovieHubRepository
{
    Task<bool> SaveChangesAsync();

    Task<(IEnumerable<Movie>, PaginationMetadata)> GetMoviesAsync(string? title, string? genre, 
        int pageNumber, int pageSize);

    Task<Movie?> GetMovieAsync(int movieId, bool includeCinemas);

    Task<bool> MovieExistsAsync(int movieId);

    Task<(IEnumerable<MovieReview>, PaginationMetadata)> GetReviewsForMovieAsync(int movieId, int pageNumber, 
        int pageSize);

    Task<MovieReview?> GetReviewForMovieAsync(int movieId, int movieReviewId);

    Task AddMovieReviewAsync(int movieId, MovieReview movieReview);

    void DeleteMovieReview(MovieReview movieReview);
}