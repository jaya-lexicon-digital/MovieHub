using MovieHub.API.Models;

namespace MovieHub.API.Services;

public interface ICinemaProvider
{
    public Task<ICollection<CinemaDto>> GetCinemasForMovieAsync(string providerMovieId);
}