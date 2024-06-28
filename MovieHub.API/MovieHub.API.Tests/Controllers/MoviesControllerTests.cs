using System.Net;
using System.Net.Http.Json;
using MovieHub.API.Models;
using MovieHub.API.Services;
using MovieHub.API.Tests.Setup;
using Newtonsoft.Json;

namespace MovieHub.API.Tests.Controllers;

[Collection("MoviesControllerTests")]
public class MoviesControllerTests : IClassFixture<TestingWebAppFactory<Program>>
{
    private readonly HttpClient _client;
    private const string ControllerUriPath = "api/movies";
    
    public MoviesControllerTests(TestingWebAppFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task Should_GET_All_Movies()
    {
        // Act
        var response = await _client.GetAsync($"{ControllerUriPath}");

        // Assert
        response.EnsureSuccessStatusCode();
        var movies = await response.Content.ReadFromJsonAsync<List<MovieDto>>();

        IEnumerable<string> values;
        PaginationMetadata? paginationMetadata = null;
        if (response.Headers.TryGetValues("X-Pagination", out values))
        {
            paginationMetadata = JsonConvert.DeserializeObject<PaginationMetadata>(values.First());
        }

        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        Assert.Equal(1, paginationMetadata.CurrentPage);
        Assert.Equal(20, paginationMetadata.PageSize);
        Assert.Equal(3, paginationMetadata.TotalItemCount);
        Assert.Equal(1, paginationMetadata.TotalPageCount);
        Assert.Equal(3, movies!.Count);
    }
    
    [Fact]
    public async Task Should_GET_Movies_By_Title()
    {
        // Act
        var response = await _client.GetAsync($"{ControllerUriPath}?title=Star W");

        // Assert
        response.EnsureSuccessStatusCode();
        var movies = await response.Content.ReadFromJsonAsync<List<MovieDto>>();

        Assert.Single(movies!);
        Assert.Equal("Star Wars", movies[0].Title);
    }
    
    [Fact]
    public async Task Should_GET_Movies_By_Genre()
    {
        // Act
        var response = await _client.GetAsync($"{ControllerUriPath}?genre=Fant");

        // Assert
        response.EnsureSuccessStatusCode();
        var movies = await response.Content.ReadFromJsonAsync<List<MovieDto>>();

        Assert.Single(movies!);
        Assert.Equal("Fantasy", movies[0].Genre);
    }
    
    [Fact]
    public async Task Should_GET_Movie_Details_By_Id()
    {
        // Act
        var response = await _client.GetAsync($"{ControllerUriPath}/1");

        // Assert
        response.EnsureSuccessStatusCode();
        var movie = await response.Content.ReadFromJsonAsync<MovieDto>();
        
        Assert.Equal("Star Wars", movie.Title);
        Assert.Equal(DateOnly.Parse("2024-01-01"), movie.ReleaseDate);
        Assert.Equal("Fantasy", movie.Genre);
        Assert.Equal(90, movie.Runtime);
        Assert.Equal("Synopsis", movie.Synopsis);
        Assert.Equal("Director", movie.Director);
        Assert.Equal("Rating", movie.Rating);
        Assert.Equal("PrincessTheatreMovieId", movie.PrincessTheatreMovieId);
        Assert.Equal(2, movie.Cinemas.Count);
        Assert.Equal("Village - Hoppers Crossing", movie.Cinemas.ElementAt(0).Name);
        Assert.Equal("Cnr Derrimut Road &, Heaths Rd, Hoppers Crossing VIC 3030", movie.Cinemas.ElementAt(0).Location);
        Assert.Equal(DateOnly.Parse("2024-07-13"), movie.Cinemas.ElementAt(0).Showtime);
        Assert.Equal(25m, movie.Cinemas.ElementAt(0).TicketPrice);
    }
    
    [Fact]
    public async Task Should_GET_Movie_Details_By_Id_Not_Found()
    {
        // Act
        var response = await _client.GetAsync($"{ControllerUriPath}/-1");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound , response.StatusCode);
    }
}