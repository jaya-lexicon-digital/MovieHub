using Polly;
using Polly.Retry;

namespace MovieHub.API.Services;

public class ResilientHttpClient
{
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly ILogger<ResilientHttpClient> _logger;
    private readonly int _maxAttempts;

    public ResilientHttpClient(ILogger<ResilientHttpClient> logger, int maxAttempts = 3,
        int exponentialBackoffInSeconds = 2)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _retryPolicy = Policy.Handle<HttpRequestException>().WaitAndRetryAsync(maxAttempts,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(exponentialBackoffInSeconds, retryAttempt)));
        _maxAttempts = maxAttempts;
    }

    public async Task<HttpResponseMessage?> GetAsync(string? requestUri, Dictionary<string, string> headers)
    {
        HttpResponseMessage? response = null;
        var numberOfAttempts = 1;

        await _retryPolicy.ExecuteAsync(async () =>
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    foreach (var keyValuePair in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(keyValuePair.Key, keyValuePair.Value);
                    }

                    response = await httpClient.GetAsync(requestUri);
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning($"HTTP GET request: {requestUri}\nfailed: {ex.Message}" +
                                       $"\nAttempts ({numberOfAttempts}/{_maxAttempts}) before reaching max retries.");
                    numberOfAttempts++;
                    throw;
                }
            }
        });

        return response;
    }
}