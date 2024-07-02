using System.ComponentModel.DataAnnotations;

namespace MovieHub.API.Models;

public class MovieReviewForCreationDto
{
    [Required(ErrorMessage = "You should provide a score value.")]
    [Range(0, 100)] 
    public decimal? Score { get; set; }
    
    public string? Comment { get; set; } = string.Empty;
    
    [Required]
    public DateTime? ReviewDate { get; set; }
}