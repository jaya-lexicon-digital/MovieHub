using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieHub.API.Entities;

public class MovieCinema
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    
    [ForeignKey("movieId")]
    [Required]
    public Movie? Movie { get; set; }
    
    [ForeignKey("cinemaId")]
    [Required]
    public Cinema? Cinema { get; set; }
    
    [Required]
    [Column("showtime")]
    public DateOnly Showtime { get; set; }
    
    [Required]
    [Column("ticketPrice")]
    public decimal TicketPrice { get; set; }
}