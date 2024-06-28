using Microsoft.EntityFrameworkCore;
using MovieHub.API.Entities;

namespace MovieHub.API.DbContexts;

public class MovieHubDbContext : DbContext
{
    public DbSet<Movie> Movie { get; set; }
    public DbSet<Cinema> Cinema { get; set; }
    public DbSet<MovieCinema> MovieCinema { get; set; }

    public MovieHubDbContext(DbContextOptions<MovieHubDbContext> options)
        : base(options)
    {
    }
}