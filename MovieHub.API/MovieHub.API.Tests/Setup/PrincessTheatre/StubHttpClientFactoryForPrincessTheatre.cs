using System.Net;

namespace MovieHub.API.Tests.Setup.PrincessTheatre;

public class StubHttpClientFactoryForPrincessTheatre : StubHttpClientFactory
{
    public StubHttpClientFactoryForPrincessTheatre() : base("https://challenge.lexicondigital.com.au/")
    {
        SetupStubScenarios();
    }

    private void SetupStubScenarios()
    {
        StubHttpRequest("/api/v2/cinemaworld/movies",
            HttpStatusCode.OK,
            SampleDataPrincessTheatre.GetDefaultCinemaProviderDto());

        StubHttpRequest("/api/v2/filmworld/movies",
            HttpStatusCode.OK,
            SampleDataPrincessTheatre.GetDefaultCinemaProviderDto(
                provider: "Film World",
                movies: [SampleDataPrincessTheatre.GetDefaultMovieFromProviderDto(id: "fwPrincessTheatreMovieId")]
            ));
    }
}