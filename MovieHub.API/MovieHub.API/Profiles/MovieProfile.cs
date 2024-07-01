using AutoMapper;

namespace MovieHub.API.Profiles;

public class MovieProfile : Profile
{
    public MovieProfile()
    {
        CreateMap<Entities.Movie, Models.MovieWithoutCinemasDto>()
            .ForMember(
                dest => dest.AverageMovieReviewScore,
                opt => opt.MapFrom(m =>
                    (double?)Math.Round(m.MovieReviews.DefaultIfEmpty().Average(mr => mr!.Score), 2)));
        CreateMap<Entities.Movie, Models.MovieDto>()
            .ForMember(
                dest => dest.AverageMovieReviewScore,
                opt => opt.MapFrom(m =>
                    (double?)Math.Round(m.MovieReviews.DefaultIfEmpty().Average(mr => mr!.Score), 2)));
    }
}