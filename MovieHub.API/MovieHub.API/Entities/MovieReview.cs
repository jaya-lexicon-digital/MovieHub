using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MovieHub.API.Entities;

public class MovieReview
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    
    [ForeignKey("movieId")]
    [Required]
    public Movie? Movie { get; set; }
    
    [Required]
    [Precision(4,2)]
    [Column("score")]
    public decimal Score { get; set; }
    
    [Column("comment")]
    public string? Comment { get; set; }
    
    [Required]
    [Column("reviewDate")]
    public DateTime ReviewDate { get; set; }
}