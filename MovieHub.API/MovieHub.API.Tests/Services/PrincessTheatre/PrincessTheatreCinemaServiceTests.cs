using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MovieHub.API.Services.PrincessTheatre;
using MovieHub.API.Services.PrincessTheatre.Config;
using MovieHub.API.Services.PrincessTheatre.Models;
using MovieHub.API.Tests.Setup;
using MovieHub.API.Tests.Setup.PrincessTheatre;

namespace MovieHub.API.Tests.Services.PrincessTheatre;

[Collection("PrincessTheatreCinemaServiceTests")]
public class PrincessTheatreCinemaServiceTests
{
    private readonly PrincessTheatreCinemaService _cinemaService;
    private readonly StubHttpClientFactory _httpClientFactory;

    public PrincessTheatreCinemaServiceTests()
    {
        _httpClientFactory = new StubHttpClientFactory();
        var mockOptions = new Mock<IOptions<PrincessTheatreOptions>>();
        mockOptions.Setup(x => x.Value).Returns(
            SampleDataPrincessTheatre.GetDefaultPrincessTheatreOptions()
        );
        var mockLogger = new Mock<ILogger<PrincessTheatreCinemaService>>();

        _cinemaService =
            new PrincessTheatreCinemaService(mockOptions.Object, mockLogger.Object, _httpClientFactory.CreateClient());
    }

    private void SetupDefaultHttpClientStubs()
    {
        _httpClientFactory.StubHttpRequest(SampleDataPrincessTheatre.UriForCinemaworld, HttpStatusCode.OK,
            SampleDataPrincessTheatre.GetDefaultCinemaProviderDto());
        _httpClientFactory.StubHttpRequest(SampleDataPrincessTheatre.UriForFilmWorld, HttpStatusCode.OK,
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
        var cinemas = await _cinemaService.GetCinemasForMovieAsync("PrincessTheatreMovieId");
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
        _httpClientFactory.StubHttpRequest(SampleDataPrincessTheatre.UriForCinemaworld,
            HttpStatusCode.OK,
            SampleDataPrincessTheatre.GetDefaultCinemaProviderDto());
        _httpClientFactory.StubHttpRequest(SampleDataPrincessTheatre.UriForFilmWorld,
            HttpStatusCode.OK,
            SampleDataPrincessTheatre.GetDefaultCinemaProviderDto(
                provider: "Film World",
                movies: [SampleDataPrincessTheatre.GetDefaultMovieFromProviderDto(id: "-1")]
            ));

        // Act
        var cinemas = await _cinemaService.GetCinemasForMovieAsync("PrincessTheatreMovieId");

        // Assert
        Assert.Single(cinemas);
    }

    [Fact]
    public async void Get_Cinemas_For_Movie_With_A_Movie_That_Does_Not_Exist()
    {
        // Arrange
        SetupDefaultHttpClientStubs();

        // Act
        var cinemas = await _cinemaService.GetCinemasForMovieAsync("-1");

        // Assert
        Assert.Empty(cinemas);
    }

    [Fact]
    public async void Get_Cinemas_For_Movie_With_One_Movie_Provider_Failing()
    {
        // Arrange
        _httpClientFactory.StubHttpRequest(SampleDataPrincessTheatre.UriForCinemaworld, HttpStatusCode.OK,
            SampleDataPrincessTheatre.GetDefaultCinemaProviderDto());
        _httpClientFactory.StubHttpRequest<CinemaProviderDto>(SampleDataPrincessTheatre.UriForFilmWorld,
            HttpStatusCode.InternalServerError, null!);

        // Act
        var cinemas = await _cinemaService.GetCinemasForMovieAsync("PrincessTheatreMovieId");

        // Assert
        Assert.Single(cinemas);
    }

    [Fact]
    public async void Get_Cinemas_For_Movie_With_All_Movie_Providers_Failing()
    {
        // Arrange
        _httpClientFactory.StubHttpRequest<CinemaProviderDto>(SampleDataPrincessTheatre.UriForCinemaworld,
            HttpStatusCode.Forbidden, null!);
        _httpClientFactory.StubHttpRequest<CinemaProviderDto>(SampleDataPrincessTheatre.UriForFilmWorld,
            HttpStatusCode.InternalServerError, null!);

        // Act
        var cinemas = await _cinemaService.GetCinemasForMovieAsync("PrincessTheatreMovieId");

        // Assert
        Assert.Empty(cinemas);
    }
}