namespace MovieHub.API.Models;

public class MoviesDto
{
    public ICollection<MovieWithoutCinemasDto> Movies { get; set; } = new List<MovieWithoutCinemasDto>();
}