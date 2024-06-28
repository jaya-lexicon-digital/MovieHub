namespace MovieHub.API.Models;

public class MovieWithoutCinemasDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateOnly ReleaseDate { get; set; }
    public string Genre { get; set; } = string.Empty;
    public int Runtime { get; set; }
    public string Synopsis { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public string Rating { get; set; } = string.Empty;
    public string PrincessTheatreMovieId { get; set; } = string.Empty;
}