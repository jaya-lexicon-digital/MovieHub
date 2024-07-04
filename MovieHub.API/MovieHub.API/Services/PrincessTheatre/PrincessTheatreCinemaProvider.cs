using MovieHub.API.Models;
using MovieHub.API.Services.PrincessTheatre.Models;
using Newtonsoft.Json;

namespace MovieHub.API.Services.PrincessTheatre;

public class PrincessTheatreCinemaProvider : ICinemaProvider
{
    private readonly ILogger<PrincessTheatreCinemaProvider> _logger;
    private readonly IHttpClient _httpClient;
    private readonly Dictionary<string, string> _movieProviders;
    private readonly Dictionary<string, string> _defaultHeaders;

    public PrincessTheatreCinemaProvider(IConfiguration configuration, ILogger<PrincessTheatreCinemaProvider> logger,
        IHttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _movieProviders = configuration.GetSection("CinemaProvider:PrincessTheatre:MovieProviders").GetChildren()
            .ToDictionary(x => x.Key, x => x.Value!);
        _defaultHeaders = new Dictionary<string, string>()
        {
            { "x-api-key", configuration["CinemaProvider:PrincessTheatre:ApiKey"]!}
        };
    }

    public async Task<ICollection<CinemaDto>> GetCinemasForMovieAsync(string providerMovieId)
    {
        var cinemas = new List<CinemaDto>();
        foreach (var keyValuePair in _movieProviders)
        {
            var cinemaProviderDto = await GetMoviesFromProviderAsync(keyValuePair.Key, keyValuePair.Value);
            var movieFromProvider = cinemaProviderDto?.Movies.FirstOrDefault(m => m.Id.Substring(2).Equals(providerMovieId));
            if (cinemaProviderDto != null && movieFromProvider != null)
            {
                cinemas.Add(ConvertMovieFromProviderToCinemaDto(cinemaProviderDto.Provider, movieFromProvider));
            }
        }
        
        return cinemas;
    }

    private async Task<CinemaProviderDto?> GetMoviesFromProviderAsync(string movieProvider, string requestUri)
    {
        CinemaProviderDto? cinemaProviderDto = null;
        try
        {
            var response = await _httpClient.GetAsync(requestUri, _defaultHeaders);
            var responseContent = await response!.Content.ReadAsStringAsync();
            cinemaProviderDto = JsonConvert.DeserializeObject<CinemaProviderDto>(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Failed to get movies from provider: {movieProvider}, requestUri: {requestUri}", ex);
        }
        
        return cinemaProviderDto!;
    }

    private CinemaDto ConvertMovieFromProviderToCinemaDto(string provider, MovieFromProviderDto movie)
    {
        return new CinemaDto()
        {
            Name = provider,
            Location = $"The address of {provider}",
            Showtime = DateOnly.FromDateTime(DateTime.Now),
            TicketPrice = movie.Price
        };
    }
}