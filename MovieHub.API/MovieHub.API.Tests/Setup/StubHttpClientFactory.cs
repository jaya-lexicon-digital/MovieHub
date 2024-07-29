using System.Net;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace MovieHub.API.Tests.Setup;

public class StubHttpClientFactory : IHttpClientFactory
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<IHttpClientFactory> _mockFactory;
    
    public StubHttpClientFactory(string? baseAddressUri = null)
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var client = new HttpClient(_mockHttpMessageHandler.Object);
        if (baseAddressUri != null) client.BaseAddress = new Uri(baseAddressUri);
        _mockFactory = new Mock<IHttpClientFactory>();
        _mockFactory.Setup(e => e.CreateClient(It.IsAny<string>())).Returns(client);
    }

    public HttpClient CreateClient(string name)
    { 
        return _mockFactory.Object.CreateClient();
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