using Polly;
using Polly.Retry;

namespace MovieHub.API.Services;

public class ResilientHttpClient
{
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly ILogger<ResilientHttpClient> _logger;

    public ResilientHttpClient(ILogger<ResilientHttpClient> logger, int maxAttempts = 3, int exponentialBackoffInSeconds = 2)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _retryPolicy = Policy.Handle<HttpRequestException>().WaitAndRetryAsync(maxAttempts,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(exponentialBackoffInSeconds, retryAttempt)));
    }

    public async Task<HttpResponseMessage?> GetAsync(string? requestUri, Dictionary<string,string> headers)
    {
        HttpResponseMessage? response = null;
        var retryRequestUri = requestUri;
        
        using (var httpClient = new HttpClient())
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    foreach (var keyValuePair in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(keyValuePair.Key, keyValuePair.Value);
                    }
                    response = await httpClient.GetAsync(retryRequestUri);
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning($"HTTP GET request: {retryRequestUri} \nfailed: {ex.Message}");
                    
                    // 'retryRequestUri' is my attempt to ensure a unique Uri as I noticed for a 403 response it was
                    // always being cached incorrectly and hence future polly retry attempts would always fail!
                    retryRequestUri = $"{requestUri}?retryTime={DateTime.Now:HH:mm:ss}";
                    
                    // TODO: the above comment doesn't actually work, hence need another way to recover from a 403.
                    // However a brand new request of this.GetAsync(..) allows for different results other than 403. 
                    
                    throw;
                }
            });
        }

        return response;
    }
}