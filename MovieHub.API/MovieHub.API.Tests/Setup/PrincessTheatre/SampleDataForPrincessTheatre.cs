using MovieHub.API.Services.PrincessTheatre.Models;

namespace MovieHub.API.Tests.Setup.PrincessTheatre;

public static class SampleDataPrincessTheatre
{
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