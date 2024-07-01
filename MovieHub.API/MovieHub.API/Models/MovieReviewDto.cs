namespace MovieHub.API.Models;

public class MovieReviewDto
{
    public int Id { get; set; }
    public decimal Score { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime ReviewDate { get; set; }
}