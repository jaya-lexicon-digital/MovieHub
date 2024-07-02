namespace MovieHub.API.Models;

public class MovieReviewsDto
{
    public ICollection<MovieReviewDto> MovieReviews { get; set; } = new List<MovieReviewDto>();
}