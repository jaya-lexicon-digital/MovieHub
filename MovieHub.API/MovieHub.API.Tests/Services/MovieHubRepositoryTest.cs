using MovieHub.API.DbContexts;
using MovieHub.API.Services;
using MovieHub.API.Tests.Setup;

namespace MovieHub.API.Tests.Services;

[Collection("MovieHubRepositoryTest")]
public class MovieHubRepositoryTest: IClassFixture<TestingWebAppFactory<Program>>
{
    private IMovieHubRepository _repository;
    private static readonly MovieHubDbContext MovieHubDbContext = SampleData.GetDefaultMovieHubDbContext();

    public MovieHubRepositoryTest()
    {
        _repository = new MovieHubRepository(MovieHubDbContext);
    }
    
    [Fact]
    public async void Get_All_Movies()
    {
        // Act
        var (movieEntities, paginationMetadata) = await _repository.GetMoviesAsync(null, null,1,20);
        var moviesList = movieEntities.ToList();
        var movie = moviesList.First();

        // Assert
        Assert.Equal(1, paginationMetadata.CurrentPage);
        Assert.Equal(20, paginationMetadata.PageSize);
        Assert.Equal(3, paginationMetadata.TotalItemCount);
        Assert.Equal(1, paginationMetadata.TotalPageCount);
        Assert.Equal(3, moviesList.Count());
        Assert.Equal(2, movie.Id);
        Assert.Equal("Die Hard", movie.Title);
        Assert.Equal(DateOnly.Parse("2024-01-01"), movie.ReleaseDate);
        Assert.Equal("Action", movie.Genre);
        Assert.Equal(90, movie.Runtime);
        Assert.Equal("Synopsis", movie.Synopsis);
        Assert.Equal("Director", movie.Director);
        Assert.Equal("Rating", movie.Rating);
        Assert.Equal("PrincessTheatreMovieId", movie.PrincessTheatreMovieId);
    }
    
    [Fact]
    public async void Get_All_Movies_None_Found()
    {
        // Act
        var (movieEntities, paginationMetadata) = await _repository.GetMoviesAsync("NOT_FOUND", "NOT_FOUND",1,20);
        
        // Assert
        Assert.Equal(1, paginationMetadata.CurrentPage);
        Assert.Equal(20, paginationMetadata.PageSize);
        Assert.Equal(0, paginationMetadata.TotalItemCount);
        Assert.Equal(0, paginationMetadata.TotalPageCount);
        Assert.Empty(movieEntities.ToList());
    }
    
    [Fact]
    public async void Get_All_Movies_By_Title()
    {
        // Act
        var (movieEntities, paginationMetadata) = await _repository.GetMoviesAsync("Star W", null,1, 20);
        var movies = movieEntities.ToList();
        var movie = movies.First();

        // Assert
        Assert.Equal(1, paginationMetadata.CurrentPage);
        Assert.Equal(20, paginationMetadata.PageSize);
        Assert.Equal(1, paginationMetadata.TotalItemCount);
        Assert.Equal(1, paginationMetadata.TotalPageCount);
        Assert.Single(movies);
        Assert.Equal(1, movie.Id);
        Assert.Equal("Star Wars", movie.Title);
        Assert.Equal(DateOnly.Parse("2024-01-01"), movie.ReleaseDate);
        Assert.Equal("Fantasy", movie.Genre);
        Assert.Equal(90, movie.Runtime);
        Assert.Equal("Synopsis", movie.Synopsis);
        Assert.Equal("Director", movie.Director);
        Assert.Equal("Rating", movie.Rating);
        Assert.Equal("PrincessTheatreMovieId", movie.PrincessTheatreMovieId);
    }
    
    [Fact]
    public async void Get_All_Movies_By_Genre()
    {
        // Act
        var (movieEntities, paginationMetadata) = await _repository.GetMoviesAsync(null, "Com",1, 20);
        var movies = movieEntities.ToList();
        var movie = movies.First();

        // Assert
        Assert.Equal(1, paginationMetadata.CurrentPage);
        Assert.Equal(20, paginationMetadata.PageSize);
        Assert.Equal(1, paginationMetadata.TotalItemCount);
        Assert.Equal(1, paginationMetadata.TotalPageCount);
        Assert.Single(movies);
        Assert.Equal(3, movie.Id);
        Assert.Equal("The Hangover", movie.Title);
        Assert.Equal(DateOnly.Parse("2024-01-01"), movie.ReleaseDate);
        Assert.Equal("Comedy", movie.Genre);
        Assert.Equal(90, movie.Runtime);
        Assert.Equal("Synopsis", movie.Synopsis);
        Assert.Equal("Director", movie.Director);
        Assert.Equal("Rating", movie.Rating);
        Assert.Equal("PrincessTheatreMovieId", movie.PrincessTheatreMovieId);
    }
    
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async void Get_Movie_Details(bool includeCinemas)
    {
        // Act
        var movie = await _repository.GetMovieAsync(1, true);

        // Assert
        Assert.Equal(1, movie!.Id);
        Assert.Equal("Star Wars", movie.Title);
        Assert.Equal(DateOnly.Parse("2024-01-01"), movie.ReleaseDate);
        Assert.Equal("Fantasy", movie.Genre);
        Assert.Equal(90, movie.Runtime);
        Assert.Equal("Synopsis", movie.Synopsis);
        Assert.Equal("Director", movie.Director);
        Assert.Equal("Rating", movie.Rating);
        Assert.Equal("PrincessTheatreMovieId", movie.PrincessTheatreMovieId);
        
        if (includeCinemas)
        {
            var movieCinema = movie.MovieCinemas.First();
            Assert.Equal(2, movie.MovieCinemas.Count);
            Assert.Equal("Star Wars", movieCinema.Movie!.Title);
            Assert.Equal("Village - Hoppers Crossing", movieCinema.Cinema!.Name);
            Assert.Equal(DateOnly.Parse("2024-07-13"), movieCinema.Showtime);
            Assert.Equal(25m, movieCinema.TicketPrice);
        }
    }
    
    [Fact]
    public async void Get_Movie_Details_Not_Found()
    {
        // Act
        var movie = await _repository.GetMovieAsync(-1, true);

        // Assert
        Assert.Null(movie);
    }
}