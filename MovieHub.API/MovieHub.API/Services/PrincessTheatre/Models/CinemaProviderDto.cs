namespace MovieHub.API.Services.PrincessTheatre.Models;

public class CinemaProviderDto
{
    public string Provider { get; set; } = string.Empty;
    public ICollection<MovieFromProviderDto> Movies { get; set; } = new List<MovieFromProviderDto>();
}