using AutoMapper;

namespace MovieHub.API.Profiles;

public class MovieCinemaProfile : Profile
{
    public MovieCinemaProfile()
    {
        CreateMap<Entities.MovieCinema, Models.CinemaDto>()
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(mc => mc.Cinema!.Name))
            .ForMember(
                dest => dest.Location,
                opt => opt.MapFrom(mc => mc.Cinema!.Location));
    }
}