using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MovieHub.API.Services.PrincessTheatre;
using MovieHub.API.Services.PrincessTheatre.Models;
using MovieHub.API.Tests.Setup;
using MovieHub.API.Tests.Setup.PrincessTheatre;

namespace MovieHub.API.Tests.Services.PrincessTheatre;

[Collection("PrincessTheatreCinemaProviderTests")]
public class PrincessTheatreCinemaProviderTests
{
    private readonly PrincessTheatreCinemaProvider _cinemaProvider;
    private readonly StubHttpClient _httpClient;

    public PrincessTheatreCinemaProviderTests()
    {
        _httpClient = new StubHttpClient();
        IConfiguration configuration = GetConfiguration();
        Mock<ILogger<PrincessTheatreCinemaProvider>> mockLogger = new Mock<ILogger<PrincessTheatreCinemaProvider>>();

        _cinemaProvider = new PrincessTheatreCinemaProvider(configuration, mockLogger.Object, _httpClient);
    }

    private IConfiguration GetConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {
                "CinemaProvider:PrincessTheatre:ApiKey", "ApiKey"
            },
            {
                "CinemaProvider:PrincessTheatre:MovieProviders:filmworld",
                "https://test.filmworld.com.au/api/v2/filmworld/movies"
            },
            {
                "CinemaProvider:PrincessTheatre:MovieProviders:cinemaworld",
                "https://test.cinemaworld.com.au/api/v2/cinemaworld/movies"
            }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
        return configuration;
    }

    private void SetupDefaultHttpClientStubs()
    {
        _httpClient.StubHttpRequest("https://test.cinemaworld.com.au/api/v2/cinemaworld/movies",
            HttpStatusCode.OK,
            SampleDataPrincessTheatre.GetDefaultCinemaProviderDto());
        _httpClient.StubHttpRequest("https://test.filmworld.com.au/api/v2/filmworld/movies",
            HttpStatusCode.OK,
            SampleDataPrincessTheatre.GetDefaultCinemaProviderDto(
                provider: "Film World",
                movies: [SampleDataPrincessTheatre.GetDefaultMovieFromProviderDto(id: "fwPrincessTheatreMovieId")]
            ));
    }

    [Fact]
    public async void Get_Cinemas_For_Movie()
    {
        // Arrange
        SetupDefaultHttpClientStubs();

        // Act
        var cinemas = await _cinemaProvider.GetCinemasForMovieAsync("PrincessTheatreMovieId");
        var cinema = cinemas.First();

        // Assert
        Assert.Equal(2, cinemas.Count());
        Assert.Equal("Cinema World", cinema.Name);
        Assert.Equal("The address of Cinema World", cinema.Location);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Now), cinema.Showtime);
        Assert.Equal(20m, cinema.TicketPrice);
    }

    [Fact]
    public async void Get_Cinemas_For_Movie_With_One_Movie_Provider_Not_Having_That_Movie()
    {
        // Arrange
        _httpClient.StubHttpRequest("https://test.cinemaworld.com.au/api/v2/cinemaworld/movies",
            HttpStatusCode.OK,
            SampleDataPrincessTheatre.GetDefaultCinemaProviderDto());
        _httpClient.StubHttpRequest("https://test.filmworld.com.au/api/v2/filmworld/movies",
            HttpStatusCode.OK,
            SampleDataPrincessTheatre.GetDefaultCinemaProviderDto(
                provider: "Film World",
                movies: [SampleDataPrincessTheatre.GetDefaultMovieFromProviderDto(id: "-1")]
            ));

        // Act
        var cinemas = await _cinemaProvider.GetCinemasForMovieAsync("PrincessTheatreMovieId");

        // Assert
        Assert.Single(cinemas);
    }

    [Fact]
    public async void Get_Cinemas_For_Movie_With_A_Movie_That_Does_Not_Exist()
    {
        // Arrange
        SetupDefaultHttpClientStubs();

        // Act
        var cinemas = await _cinemaProvider.GetCinemasForMovieAsync("-1");

        // Assert
        Assert.Empty(cinemas);
    }

    [Fact]
    public async void Get_Cinemas_For_Movie_With_One_Movie_Provider_Failing()
    {
        // Arrange
        _httpClient.StubHttpRequest("https://test.cinemaworld.com.au/api/v2/cinemaworld/movies",
            HttpStatusCode.OK,
            SampleDataPrincessTheatre.GetDefaultCinemaProviderDto());
        _httpClient.StubHttpRequest<CinemaProviderDto>("https://test.filmworld.com.au/api/v2/filmworld/movies",
            HttpStatusCode.InternalServerError, null!);

        // Act
        var cinemas = await _cinemaProvider.GetCinemasForMovieAsync("PrincessTheatreMovieId");

        // Assert
        Assert.Single(cinemas);
    }

    [Fact]
    public async void Get_Cinemas_For_Movie_With_All_Movie_Providers_Failing()
    {
        // Arrange
        _httpClient.StubHttpRequest<CinemaProviderDto>("https://test.cinemaworld.com.au/api/v2/cinemaworld/movies",
            HttpStatusCode.Forbidden, null!);
        _httpClient.StubHttpRequest<CinemaProviderDto>("https://test.filmworld.com.au/api/v2/filmworld/movies",
            HttpStatusCode.InternalServerError, null!);

        // Act
        var cinemas = await _cinemaProvider.GetCinemasForMovieAsync("PrincessTheatreMovieId");

        // Assert
        Assert.Empty(cinemas);
    }
}