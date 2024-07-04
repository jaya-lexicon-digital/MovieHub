using System.Text.Json.Serialization;

namespace MovieHub.API.Services.PrincessTheatre.Models;

public class MovieFromProviderDto
{
    [JsonPropertyName("ID")]
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Poster { get; set; } = string.Empty;
    public string Actors { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0;
}