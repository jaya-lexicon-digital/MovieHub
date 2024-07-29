namespace MovieHub.API.Services.PrincessTheatre.Config;

public class PrincessTheatreOptions
{
    public const string PrincessTheatre = "CinemaService:PrincessTheatre";
    public Dictionary<string, string> DefaultHeaders { get; set; } = new Dictionary<string, string>();
    public List<MovieProvider> MovieProviders { get; set; } = new List<MovieProvider>();
}

public class MovieProvider
{
    public string Name { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}