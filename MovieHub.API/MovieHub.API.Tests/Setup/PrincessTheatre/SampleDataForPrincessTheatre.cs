using MovieHub.API.Services.PrincessTheatre.Config;
using MovieHub.API.Services.PrincessTheatre.Models;

namespace MovieHub.API.Tests.Setup.PrincessTheatre;

public static class SampleDataPrincessTheatre
{
    public const string UriForFilmWorld = "https://test.filmworld.com.au/api/v2/filmworld/movies";
    public const string UriForCinemaworld = "https://test.cinemaworld.com.au/api/v2/cinemaworld/movies";

    public static PrincessTheatreOptions GetDefaultPrincessTheatreOptions()
    {
        return new PrincessTheatreOptions()
        {
            DefaultHeaders =
            {
                { "apiKey", "apiKeyValue123" }
            },
            MovieProviders =
            {
                new MovieProvider()
                {
                    Name = "cinemaworld",
                    Uri = UriForCinemaworld,
                    Location = "The address of Cinema World"
                },
                new MovieProvider()
                {
                    Name = "filmworld",
                    Uri = UriForFilmWorld,
                    Location = "The address of Film World"
                }
            }
        };
    }
    public static MovieFromProviderDto GetDefaultMovieFromProviderDto(
        string id = "cmPrincessTheatreMovieId",
        string title = "Title",
        string type = "Type",
        string poster = "Poster",
        string actors = "Actors",
        decimal price = 20m)
    {
        return new MovieFromProviderDto()
        {
            Id = id,
            Title = title,
            Type = type,
            Poster = poster,
            Actors = actors,
            Price = price
        };
    }
    
    public static CinemaProviderDto GetDefaultCinemaProviderDto(
        string provider = "Cinema World",
        List<MovieFromProviderDto>? movies = null)
    {
        return new CinemaProviderDto()
        {
            Provider = provider,
            Movies = movies ?? [GetDefaultMovieFromProviderDto()]
        };
    }
}