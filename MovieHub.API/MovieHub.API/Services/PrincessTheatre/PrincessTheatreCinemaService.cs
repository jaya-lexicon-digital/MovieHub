using Microsoft.Extensions.Options;
using MovieHub.API.Models;
using MovieHub.API.Services.PrincessTheatre.Config;
using MovieHub.API.Services.PrincessTheatre.Models;
using Newtonsoft.Json;

namespace MovieHub.API.Services.PrincessTheatre;

public class PrincessTheatreCinemaService : ICinemaService
{
    private readonly PrincessTheatreOptions _options;
    private readonly HttpClient _httpClient;
    private readonly ILogger<PrincessTheatreCinemaService> _logger;

    public PrincessTheatreCinemaService(IOptions<PrincessTheatreOptions> options, 
        ILogger<PrincessTheatreCinemaService> logger, HttpClient httpClient)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        foreach (var header in _options.DefaultHeaders)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }

    public async Task<ICollection<CinemaDto>> GetCinemasForMovieAsync(string providerMovieId)
    {
        var cinemas = new List<CinemaDto>();
        foreach (var movieProvider in _options.MovieProviders)
        {
            var cinemaProviderDto = await GetMoviesFromProviderAsync(movieProvider.Name, movieProvider.Uri);
            var movieFromProvider =
                cinemaProviderDto?.Movies.FirstOrDefault(m => m.Id.Substring(2).Equals(providerMovieId));
            if (cinemaProviderDto != null && movieFromProvider != null)
            {
                cinemas.Add(ConvertMovieFromProviderToCinemaDto(cinemaProviderDto.Provider, movieProvider.Location,
                    movieFromProvider));
            }
        }

        return cinemas;
    }

    private async Task<CinemaProviderDto?> GetMoviesFromProviderAsync(string movieProvider, string requestUri)
    {
        CinemaProviderDto? cinemaProviderDto = null;
        try
        {
            var response = await _httpClient.GetAsync(requestUri);
            var responseContent = await response.Content.ReadAsStringAsync();
            cinemaProviderDto = JsonConvert.DeserializeObject<CinemaProviderDto>(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Failed to get movies from provider: {movieProvider}, requestUri: {requestUri}" +
                                "\nexception was: {@ex}", movieProvider, requestUri, ex);
        }

        return cinemaProviderDto!;
    }

    private CinemaDto ConvertMovieFromProviderToCinemaDto(string provider, string locationOfProvider,
        MovieFromProviderDto movie)
    {
        return new CinemaDto()
        {
            Name = provider,
            Location = locationOfProvider,
            Showtime = DateOnly.FromDateTime(DateTime.Now),
            TicketPrice = movie.Price
        };
    }
}