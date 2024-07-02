using MovieHub.API.DbContexts;
using MovieHub.API.Services;
using MovieHub.API.Tests.Setup;

namespace MovieHub.API.Tests.Services;

[Collection("MovieHubRepositoryTest")]
public class MovieHubRepositoryTest : IClassFixture<TestingWebAppFactory<Program>>
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
        var (movieEntities, paginationMetadata) = await _repository.GetMoviesAsync(null, null, 1, 20);
        var moviesList = movieEntities.ToList();
        var movie = moviesList.First();

        // Assert
        Assert.Equal(1, paginationMetadata.CurrentPage);
        Assert.Equal(20, paginationMetadata.PageSize);
        Assert.Equal(5, paginationMetadata.TotalItemCount);
        Assert.Equal(1, paginationMetadata.TotalPageCount);
        Assert.Equal(5, moviesList.Count());
        Assert.Equal(2, movie.Id);
        Assert.Equal("Die Hard", movie.Title);
        Assert.Equal(DateOnly.Parse("2024-01-01"), movie.ReleaseDate);
        Assert.Equal("Action", movie.Genre);
        Assert.Equal(90, movie.Runtime);
        Assert.Equal("Synopsis", movie.Synopsis);
        Assert.Equal("Director", movie.Director);
        Assert.Equal("Rating", movie.Rating);
        Assert.Equal("PrincessTheatreMovieId", movie.PrincessTheatreMovieId);
        Assert.Empty(movie.MovieReviews);
    }

    [Fact]
    public async void Get_All_Movies_None_Found()
    {
        // Act
        var (movieEntities, paginationMetadata) = await _repository.GetMoviesAsync("NOT_FOUND", "NOT_FOUND", 1, 20);

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
        var (movieEntities, paginationMetadata) = await _repository.GetMoviesAsync("Star W", null, 1, 20);
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
        Assert.Equal(3, movie.MovieReviews.Count);
    }

    [Fact]
    public async void Get_All_Movies_By_Genre()
    {
        // Act
        var (movieEntities, paginationMetadata) = await _repository.GetMoviesAsync(null, "Com", 1, 20);
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
        Assert.Empty(movie.MovieReviews);
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
        Assert.Equal(3, movie.MovieReviews.Count);

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

    [Theory]
    [InlineData(1, true)]
    [InlineData(-1, false)]
    public async void Movie_Exists(int movieId, bool expectedResult)
    {
        // Act
        var movieExists = await _repository.MovieExistsAsync(movieId);

        // Assert
        Assert.Equal(expectedResult, movieExists);
    }

    [Fact]
    public async void Get_All_Reviews_For_Movie()
    {
        // Act
        var (movieReviewsEntities, paginationMetadata) = await _repository.GetReviewsForMovieAsync(movieId: 1, 1, 10);
        var movieReviewsList = movieReviewsEntities.ToList();
        var movieReview = movieReviewsList.First();

        // Assert
        Assert.Equal(1, paginationMetadata.CurrentPage);
        Assert.Equal(10, paginationMetadata.PageSize);
        Assert.Equal(3, paginationMetadata.TotalItemCount);
        Assert.Equal(1, paginationMetadata.TotalPageCount);

        Assert.Equal(3, movieReviewsList.Count());
        Assert.Equal(1, movieReview.Id);
        Assert.Equal(40m, movieReview.Score);
        Assert.Equal("Boring", movieReview.Comment);
        Assert.InRange(movieReview.ReviewDate, DateTime.Now.AddMinutes(-10), DateTime.Now);
        Assert.Equal("Star Wars", movieReview.Movie!.Title);
        Assert.Equal(3, movieReview.Movie.MovieReviews.Count());
    }

    [Fact]
    public async void Get_All_Reviews_For_Movie_None_Found()
    {
        // Act
        var (movieReviewsEntities, paginationMetadata) = await _repository.GetReviewsForMovieAsync(movieId: -1, 1, 10);

        // Assert
        Assert.Equal(1, paginationMetadata.CurrentPage);
        Assert.Equal(10, paginationMetadata.PageSize);
        Assert.Equal(0, paginationMetadata.TotalItemCount);
        Assert.Equal(0, paginationMetadata.TotalPageCount);
        Assert.Empty(movieReviewsEntities.ToList());
    }

    [Fact]
    public async void Get_Review_For_Movie()
    {
        // Act
        var movieReview = await _repository.GetReviewForMovieAsync(1, 2);

        // Assert
        Assert.Equal(1, movieReview!.Movie!.Id);
        Assert.Equal("Star Wars", movieReview.Movie!.Title);
        Assert.Equal(3, movieReview.Movie.MovieReviews.Count());

        Assert.Equal(2, movieReview.Id);
        Assert.Equal(90m, movieReview.Score);
        Assert.Equal("Surprisingly Good", movieReview.Comment);
        Assert.InRange(movieReview.ReviewDate, DateTime.Now.AddDays(-1).AddMinutes(-10), DateTime.Now.AddDays(-1));
    }
    
    [Theory]
    [InlineData(1, -1)]
    [InlineData(-1, 1)]
    [InlineData(-1, -1)]
    public async void Get_Review_For_Movie_Not_Found(int movieId, int movieReviewId)
    {
        // Act
        var movie = await _repository.GetReviewForMovieAsync(movieId, movieReviewId);

        // Assert
        Assert.Null(movie);
    }
    
    [Fact]
    public async void Add_And_Delete_Movie_Review()
    {
        await Test_Add_Movie_Review();
        await Test_Delete_Movie_Review();
    }

    private async Task Test_Add_Movie_Review()
    {
        // Arrange
        const int movieId = 4;
        var movie = await _repository.GetMovieAsync(movieId, false);
        var (movieReviewsEntitiesPriorToAdd, _) = await _repository.GetReviewsForMovieAsync(movie!.Id, 1, 10);
        var movieReview = SampleData.GetDefaultMovieReview(movie: movie);
        
        // Act - Add Movie Review
        await _repository.AddMovieReviewAsync(movie.Id, movieReview);
        var isSaveSuccessful = await _repository.SaveChangesAsync();
        var (movieReviewsEntitiesAfterAdd, _) = await _repository.GetReviewsForMovieAsync(movie.Id, 1, 10);
        var movieReviewsAfterAdd = movieReviewsEntitiesAfterAdd.ToList();
        var movieReviewAdded = movieReviewsAfterAdd.First();
        
        // Assert
        Assert.Empty(movieReviewsEntitiesPriorToAdd);
        Assert.True(isSaveSuccessful);
        Assert.Single(movieReviewsAfterAdd);
        Assert.Equal(movie.Id, movieReviewAdded.Movie!.Id);
        Assert.Single(movieReviewAdded.Movie.MovieReviews);
        Assert.NotEqual(default, movieReviewAdded.Id);
        Assert.Equal(movieReview.Score, movieReviewAdded.Score);
        Assert.Equal(movieReview.Comment, movieReviewAdded.Comment);
        Assert.Equal(movieReview.ReviewDate, movieReviewAdded.ReviewDate);
    }
    
    private async Task Test_Delete_Movie_Review()
    {
        // Arrange
        const int movieId = 4;
        var movie = await _repository.GetMovieAsync(movieId, false);
        var (movieReviewsEntitiesPriorToDelete, _) = await _repository.GetReviewsForMovieAsync(movie!.Id, 1, 10);
        var movieReviewsPriorToDelete = movieReviewsEntitiesPriorToDelete.ToList();
        var movieReview = movieReviewsPriorToDelete.First();
        
        // Act - Delete Movie Review
        _repository.DeleteMovieReview(movieReview);
        var isDeleteSuccessful = await _repository.SaveChangesAsync();
        var (movieReviewsEntitiesAfterDelete, _) = await _repository.GetReviewsForMovieAsync(movie.Id, 1, 10);
        
        // Assert
        Assert.Single(movieReviewsPriorToDelete);
        Assert.True(isDeleteSuccessful);
        Assert.Empty(movieReviewsEntitiesAfterDelete);
    }
}