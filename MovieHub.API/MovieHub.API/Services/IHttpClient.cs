namespace MovieHub.API.Services;

public interface IHttpClient
{
    public Task<HttpResponseMessage?> GetAsync(string? requestUri, Dictionary<string, string> headers);
}