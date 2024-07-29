using MovieHub.API.Models;

namespace MovieHub.API.Services;

public interface ICinemaService
{
    public Task<ICollection<CinemaDto>> GetCinemasForMovieAsync(string providerMovieId);
}