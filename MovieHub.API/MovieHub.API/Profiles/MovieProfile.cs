using AutoMapper;

namespace MovieHub.API.Profiles;

public class MovieProfile : Profile
{
    public MovieProfile()
    {
        CreateMap<Entities.Movie, Models.MovieWithoutCinemasDto>();
        CreateMap<Entities.Movie, Models.MovieDto>();
    }
}