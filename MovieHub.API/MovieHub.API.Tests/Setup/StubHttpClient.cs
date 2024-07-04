using System.Net;
using Moq;
using Moq.Protected;
using MovieHub.API.Services;
using Newtonsoft.Json;

namespace MovieHub.API.Tests.Setup;

public class StubHttpClient : IHttpClient
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    
    public StubHttpClient(string? baseAddressUri = null)
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var client = new HttpClient(_mockHttpMessageHandler.Object);
        if (baseAddressUri != null) client.BaseAddress = new Uri(baseAddressUri);
        Mock<IHttpClientFactory> mockFactory = new();
        mockFactory.Setup(e => e.CreateClient(It.IsAny<string>())).Returns(client);
        _httpClient = mockFactory.Object.CreateClient();
    }

    public async Task<HttpResponseMessage?> GetAsync(string? requestUri, Dictionary<string, string> headers)
    {
        foreach (var keyValuePair in headers)
        {
            _httpClient.DefaultRequestHeaders.Add(keyValuePair.Key, keyValuePair.Value);
        }
        var response = await _httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        return response;
    }
    
    public void StubHttpRequest<T>(string requestUrl, HttpStatusCode statusCode, T content)
    {
        var contentAsJson = (typeof(T) == typeof(string))
            ? (string)(object)content!
            : JsonConvert.SerializeObject(content);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(msg =>
                    msg.RequestUri!.ToString().EndsWith(requestUrl, StringComparison.InvariantCultureIgnoreCase)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(contentAsJson),
            });
    }
}