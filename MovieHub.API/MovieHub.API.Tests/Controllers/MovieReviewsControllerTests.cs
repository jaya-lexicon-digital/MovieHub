using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MovieHub.API.Models;
using MovieHub.API.Services;
using MovieHub.API.Tests.Setup;
using Newtonsoft.Json;

namespace MovieHub.API.Tests.Controllers;

[Collection("MovieReviewsControllerTests")]
public class MovieReviewsControllerTests : IClassFixture<TestingWebAppFactory<Program>>
{
    private readonly HttpClient _client;

    public MovieReviewsControllerTests(TestingWebAppFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task Should_GET_All_Reviews_For_Movie()
    {
        // Act
        var response = await _client.GetAsync("api/movie/1/reviews");

        // Assert
        response.EnsureSuccessStatusCode();
        var movieReviewsResponse = await response.Content.ReadFromJsonAsync<MovieReviewsDto>();
        var movieReviewDto = movieReviewsResponse!.MovieReviews.First();

        PaginationMetadata? paginationMetadata = null;
        if (response.Headers.TryGetValues("X-Pagination", out var values))
        {
            paginationMetadata = JsonConvert.DeserializeObject<PaginationMetadata>(values.First());
        }

        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        Assert.Equal(1, paginationMetadata!.CurrentPage);
        Assert.Equal(20, paginationMetadata.PageSize);
        Assert.Equal(3, paginationMetadata.TotalItemCount);
        Assert.Equal(1, paginationMetadata.TotalPageCount);
        Assert.Equal(3, movieReviewsResponse.MovieReviews.Count());
        Assert.Equal(1, movieReviewDto.Id);
        Assert.Equal(40m, movieReviewDto.Score);
        Assert.Equal("Boring", movieReviewDto.Comment);
        Assert.InRange(movieReviewDto.ReviewDate, DateTime.Now.AddMinutes(-10), DateTime.Now);
    }
    
    [Fact]
    public async Task Should_GET_All_Reviews_For_Movie_Not_Found()
    {
        // Act
        var response = await _client.GetAsync("api/movie/-1/reviews");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound , response.StatusCode);
    }
    
    [Fact]
    public async Task Should_POST_Movie_Review()
    {
        // Arrange
        var postRequest = new HttpRequestMessage(HttpMethod.Post, "api/movie/5/reviews");
        var movieReviewForCreationDto = SampleData.GetDefaultMovieReviewForCreationDto();
        var jsonData = JsonConvert.SerializeObject(movieReviewForCreationDto);
        postRequest.Content = new StringContent(jsonData, Encoding.UTF8, MediaTypeNames.Application.Json);
            
        // Act
        var response = await _client.SendAsync(postRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created , response.StatusCode);
    }
    
    [Fact]
    public async Task Should_POST_Movie_Review_Not_Found()
    {
        // Arrange
        var postRequest = new HttpRequestMessage(HttpMethod.Post, "api/movie/-1/reviews");
        var movieReviewForCreationDto = SampleData.GetDefaultMovieReviewForCreationDto();
        var jsonData = JsonConvert.SerializeObject(movieReviewForCreationDto);
        postRequest.Content = new StringContent(jsonData, Encoding.UTF8, MediaTypeNames.Application.Json);
            
        // Act
        var response = await _client.SendAsync(postRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound , response.StatusCode);
    }
    
    [Fact]
    public async Task Should_POST_Movie_Review_Bad_Request()
    {
        // Arrange
        var postRequest = new HttpRequestMessage(HttpMethod.Post, "api/movie/5/reviews");
        var movieReviewForCreationDto = SampleData.GetDefaultMovieReviewForCreationDto(score: null);
        var jsonData = JsonConvert.SerializeObject(movieReviewForCreationDto);
        postRequest.Content = new StringContent(jsonData, Encoding.UTF8, MediaTypeNames.Application.Json);
            
        // Act
        var response = await _client.SendAsync(postRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest , response.StatusCode);
        Assert.Equal((int)HttpStatusCode.BadRequest , problemDetails!.Status);
        Assert.Contains("You should provide a score value.", responseContent);
    }
    
    [Fact]
    public async Task Should_PUT_Movie_Review()
    {
        // Arrange
        var putRequest = new HttpRequestMessage(HttpMethod.Put, "api/movie/5/reviews/4");
        var movieReviewForCUpdateDto = SampleData.GetDefaultMovieReviewForUpdateDto(reviewDate: DateTime.Now);
        var jsonData = JsonConvert.SerializeObject(movieReviewForCUpdateDto);
        putRequest.Content = new StringContent(jsonData, Encoding.UTF8, MediaTypeNames.Application.Json);
            
        // Act
        var response = await _client.SendAsync(putRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent , response.StatusCode);
    }
    
    [Fact]
    public async Task Should_PUT_Movie_Review_Not_Found()
    {
        // Arrange
        var putRequest = new HttpRequestMessage(HttpMethod.Put, "api/movie/-1/reviews/4");
        var movieReviewForCUpdateDto = SampleData.GetDefaultMovieReviewForUpdateDto(reviewDate: DateTime.Now);
        var jsonData = JsonConvert.SerializeObject(movieReviewForCUpdateDto);
        putRequest.Content = new StringContent(jsonData, Encoding.UTF8, MediaTypeNames.Application.Json);
            
        // Act
        var response = await _client.SendAsync(putRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound , response.StatusCode);
    }
    
    [Fact]
    public async Task Should_PUT_Movie_Review_Bad_Request()
    {
        // Arrange
        var putRequest = new HttpRequestMessage(HttpMethod.Put, "api/movie/5/reviews/4");
        var movieReviewForCUpdateDto = SampleData.GetDefaultMovieReviewForUpdateDto(reviewDate: null);
        var jsonData = JsonConvert.SerializeObject(movieReviewForCUpdateDto);
        putRequest.Content = new StringContent(jsonData, Encoding.UTF8, MediaTypeNames.Application.Json);
            
        // Act
        var response = await _client.SendAsync(putRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest , response.StatusCode);
        Assert.Equal((int)HttpStatusCode.BadRequest , problemDetails!.Status);
        Assert.Contains("The ReviewDate field is required.", responseContent);
    }
    
    [Fact]
    public async Task Should_DELETE_Movie_Review()
    {
        // Arrange
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, "api/movie/5/reviews/5");
        
        // Act
        var response = await _client.SendAsync(deleteRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent , response.StatusCode);
    }
    
    [Fact]
    public async Task Should_DELETE_Movie_Review_Not_Found()
    {
        // Arrange
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, "api/movie/-1/reviews/5");
        
        // Act
        var response = await _client.SendAsync(deleteRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound , response.StatusCode);
    }
    
}