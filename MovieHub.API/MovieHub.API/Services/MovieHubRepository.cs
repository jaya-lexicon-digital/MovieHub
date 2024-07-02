using Microsoft.EntityFrameworkCore;
using MovieHub.API.DbContexts;
using MovieHub.API.Entities;

namespace MovieHub.API.Services;

public class MovieHubRepository : IMovieHubRepository
{
    private readonly MovieHubDbContext _context;
    
    public MovieHubRepository(MovieHubDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _context.SaveChangesAsync() >= 1);
    }
    
    public async Task<(IEnumerable<Movie>, PaginationMetadata)> GetMoviesAsync(string? title, string? genre,
        int pageNumber, int pageSize)
    {
        var collection = _context.Movie as IQueryable<Movie>;
        if (!string.IsNullOrWhiteSpace(title))
        {
            title = title.Trim();
            collection = collection.Where(m => m.Title.Contains(title));
        }
        if (!string.IsNullOrWhiteSpace(genre))
        {
            genre = genre.Trim();
            collection = collection.Where(m => m.Genre.Contains(genre));
        }

        collection = collection.Include(m => m.MovieReviews);
        var totalItemCount = await collection.CountAsync();
        var paginationMetadata = new PaginationMetadata(
            totalItemCount, pageSize, pageNumber);
        
        var collectionToReturn = await collection.OrderBy(m=> m.Title)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (collectionToReturn, paginationMetadata);
    }

    public async Task<Movie?> GetMovieAsync(int movieId, bool includeCinemas)
    {
        if (includeCinemas)
        {
            return await _context.Movie
                .Where(m => m.Id == movieId)
                .Include(m => m.MovieCinemas)
                .ThenInclude(mc => mc.Cinema)
                .Include(m => m.MovieReviews)
                .FirstOrDefaultAsync();
        }

        return await _context.Movie
            .Where(m => m.Id == movieId)
            .Include(m => m.MovieReviews)
            .FirstOrDefaultAsync();
    }
    
    public async Task<bool> MovieExistsAsync(int movieId)
    {
        return await _context.Movie.AnyAsync(m => m.Id == movieId);
    }

    public async Task<(IEnumerable<MovieReview>, PaginationMetadata)> GetReviewsForMovieAsync(int movieId, 
        int pageNumber, int pageSize)
    {
        var collection = _context.MovieReview.Where(mr => mr.Movie!.Id == movieId);
        var totalItemCount = await collection.CountAsync();
        var paginationMetadata = new PaginationMetadata(
            totalItemCount, pageSize, pageNumber);
        
        var collectionToReturn = await collection.OrderByDescending(mr => mr.ReviewDate)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();
        
        return (collectionToReturn, paginationMetadata);
    }

    public async Task<MovieReview?> GetReviewForMovieAsync(int movieId, int movieReviewId)
    {
        return await _context.MovieReview
            .Where(mr => mr.Id == movieReviewId && mr.Movie!.Id == movieId).FirstOrDefaultAsync();
    }

    public async Task AddMovieReviewAsync(int movieId, MovieReview movieReview)
    {
        var movie = await GetMovieAsync(movieId, false);
        movie?.MovieReviews.Add(movieReview);
    }

    public void DeleteMovieReview(MovieReview movieReview)
    {
        _context.MovieReview.Remove(movieReview);
    }
    
}