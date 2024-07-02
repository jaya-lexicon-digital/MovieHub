using Microsoft.EntityFrameworkCore;
using MovieHub.API.DbContexts;
using MovieHub.API.Entities;
using MovieHub.API.Models;

namespace MovieHub.API.Tests.Setup;

public class SampleData
{
    public static MovieHubDbContext GetDefaultMovieHubDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<MovieHubDbContext>();
        optionsBuilder.UseInMemoryDatabase("InMemoryMovieHubTest");
        var dbContext = new MovieHubDbContext(optionsBuilder.Options);
        PopulateDb(dbContext);

        return dbContext;
    }
    
    public static void PopulateDb(MovieHubDbContext dbContext)
    {
        var movies = new List<Movie>()
        {
            GetDefaultMovie(title: "Star Wars", genre: "Fantasy"),
            GetDefaultMovie(title: "Die Hard", genre: "Action"),
            GetDefaultMovie(title: "The Hangover", genre: "Comedy"),
            GetDefaultMovie(title: "Scream", genre: "Horror"),
            GetDefaultMovie(title: "Indiana Jones", genre: "Adventure")
        };

        var cinemas = new List<Cinema>()
        {
            GetDefaultCinema(name: "Village - Hoppers Crossing",
                "Cnr Derrimut Road &, Heaths Rd, Hoppers Crossing VIC 3030"),
            GetDefaultCinema(name: "Hoyts - Melbourne Central", "Cnr Swanston &, La Trobe St, Melbourne VIC 3000")
        };

        var movieCinemas = new List<MovieCinema>()
        {
            GetDefaultMovieCinema(movie: movies[0], cinema: cinemas[0], showtime: "2024-07-13", ticketPrice: 25m),
            GetDefaultMovieCinema(movie: movies[0], cinema: cinemas[1]),
            GetDefaultMovieCinema(movie: movies[1], cinema: cinemas[0])
        };

        var movieReviews = new List<MovieReview>()
        {
            GetDefaultMovieReview(movie: movies[0], score: 40m, comment: "Boring"),
            GetDefaultMovieReview(movie: movies[0], score: 90m, comment: "Surprisingly Good", DateTime.Now.AddDays(-1)),
            GetDefaultMovieReview(movie: movies[0], reviewDate: DateTime.Now.AddDays(-7)),
            GetDefaultMovieReview(movie: movies[4], score: 10m, comment: "Bad"),
            GetDefaultMovieReview(movie: movies[4], score: 100m, comment: "Delete Me")
        };

        dbContext.Movie.AddRange(movies);
        dbContext.Cinema.AddRange(cinemas);
        dbContext.MovieCinema.AddRange(movieCinemas);
        dbContext.MovieReview.AddRange(movieReviews);

        dbContext.SaveChanges();
    }

    public static Movie GetDefaultMovie(
        string title = "Title",
        string releaseDate = "2024-01-01",
        string genre = "Genre",
        int runtime = 90,
        string synopsis = "Synopsis",
        string director = "Director",
        string rating = "Rating",
        string princessTheatreMovieId = "PrincessTheatreMovieId")
    {
        return new Movie(title, DateOnly.Parse(releaseDate), genre, runtime, synopsis, director,
            rating, princessTheatreMovieId);
    }

    public static Cinema GetDefaultCinema(
        string name = "Name",
        string location = "Location")
    {
        return new Cinema(name, location);
    }

    public static MovieCinema GetDefaultMovieCinema(
        Movie? movie = null,
        Cinema? cinema = null,
        string showtime = "2024-07-01",
        decimal ticketPrice = 20m)
    {
        return new MovieCinema()
        {
            Movie = movie ?? GetDefaultMovie(),
            Cinema = cinema ?? GetDefaultCinema(),
            Showtime = DateOnly.Parse(showtime),
            TicketPrice = ticketPrice
        };
    }
    
    public static MovieReview GetDefaultMovieReview(
        Movie? movie = null,
        decimal score = 80,
        string? comment = "comment",
        DateTime? reviewDate = null)
    {
        return new MovieReview()
        {
            Movie = movie ?? GetDefaultMovie(),
            Score = score,
            Comment = comment,
            ReviewDate = reviewDate ?? DateTime.Now
        };
    }
    
    public static MovieReviewForCreationDto GetDefaultMovieReviewForCreationDto(
        decimal? score = 80,
        string? comment = "comment",
        DateTime? reviewDate = null)
    {
        return new MovieReviewForCreationDto()
        {
            Score = score,
            Comment = comment,
            ReviewDate = reviewDate ?? DateTime.Now
        };
    }
    
    public static MovieReviewForUpdateDto GetDefaultMovieReviewForUpdateDto(
        decimal? score = 80,
        string? comment = "comment",
        DateTime? reviewDate = null)
    {
        return new MovieReviewForUpdateDto()
        {
            Score = score,
            Comment = comment,
            ReviewDate = reviewDate
        };
    }
}