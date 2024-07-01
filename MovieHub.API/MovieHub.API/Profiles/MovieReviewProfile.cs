using AutoMapper;

namespace MovieHub.API.Profiles;

public class MovieReviewProfile : Profile
{
    public MovieReviewProfile()
    {
        CreateMap<Entities.MovieReview, Models.MovieReviewDto>();
        CreateMap<Models.MovieReviewForCreationDto, Entities.MovieReview>();
        CreateMap<Models.MovieReviewForUpdateDto, Entities.MovieReview>();
    }
}