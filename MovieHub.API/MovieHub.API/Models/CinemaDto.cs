namespace MovieHub.API.Models;

public class CinemaDto
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateOnly Showtime { get; set; }
    public decimal TicketPrice { get; set; }
}