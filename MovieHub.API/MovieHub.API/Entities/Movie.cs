using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieHub.API.Entities;

public class Movie
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(128)]
    [Column("title")]
    public string Title { get; set; }
    
    [Required]
    [Column("releaseDate")]
    public DateOnly ReleaseDate { get; set; }
    
    [Required]
    [MaxLength(64)]
    [Column("genre")]
    public string Genre { get; set; }
    
    [Required]
    [Column("runtime")]
    public int Runtime { get; set; }
    
    [Required]
    [Column("synopsis")]
    public string Synopsis { get; set; }
    
    [Required]
    [MaxLength(64)]
    [Column("director")]
    public string Director { get; set; }
    
    [Required]
    [MaxLength(8)]
    [Column("rating")]
    public string Rating { get; set; }
    
    [Required]
    [MaxLength(16)]
    [Column("princessTheatreMovieId")]
    public string PrincessTheatreMovieId { get; set; }
    
    public ICollection<MovieCinema> MovieCinemas { get; set; } = new List<MovieCinema>();
    public ICollection<MovieReview> MovieReviews { get; set; } = new List<MovieReview>();

    public Movie(string title, DateOnly releaseDate, string genre, int runtime, string synopsis, string director,
        string rating, string princessTheatreMovieId)
    {
        Title = title;
        ReleaseDate = releaseDate;
        Genre = genre;
        Runtime = runtime;
        Synopsis = synopsis;
        Director = director;
        Rating = rating;
        PrincessTheatreMovieId = princessTheatreMovieId;
    }
}