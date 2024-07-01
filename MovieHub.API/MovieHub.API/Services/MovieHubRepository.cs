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
                .Include(m => m.MovieCinemas)
                .ThenInclude(mc => mc.Cinema)
                .Where(m => m.Id == movieId)
                .FirstOrDefaultAsync();
        }

        return await _context.Movie
            .Where(m => m.Id == movieId).FirstOrDefaultAsync();
    }
    
}