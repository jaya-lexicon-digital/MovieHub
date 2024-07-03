using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MovieHub.API.Models;
using MovieHub.API.Services;

namespace MovieHub.API.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    private readonly IMovieHubRepository _movieHubRepository;
    private readonly ICinemaProvider _cinemaProvider;
    private readonly IMapper _mapper;
    private readonly ILogger<MoviesController> _logger;
    private const int MaxMoviesPageSize = 100;

    public MoviesController(IMovieHubRepository movieHubRepository, ICinemaProvider cinemaProvider,
        IMapper mapper, ILogger<MoviesController> logger)
    {
        _movieHubRepository = movieHubRepository ?? throw new ArgumentNullException(nameof(movieHubRepository));
        _cinemaProvider = cinemaProvider ?? throw new ArgumentNullException(nameof(cinemaProvider));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<MoviesDto>> GetMovies(string? title, string? genre,
        int pageNumber = 1, int pageSize = 20)
    {
        if (pageSize > MaxMoviesPageSize)
        {
            _logger.LogInformation($"Warning pageSize of: '{pageSize}' exceeds the max pageSize of: "
                                   + $"'{MaxMoviesPageSize}', hence changing pageSize to the maximum allowed!");
            pageSize = MaxMoviesPageSize;
        }
        
        var (movieEntities, paginationMetadata) = await _movieHubRepository
            .GetMoviesAsync(title, genre, pageNumber, pageSize);
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        var moviesDto = new MoviesDto()
        {
            Movies = _mapper.Map<ICollection<MovieWithoutCinemasDto>>(movieEntities)
        };
        
        return Ok(moviesDto);
    }

    [HttpGet("{movieId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<MovieDto>> GetMovie(int movieId, bool includeCinemas = true)
    {
        var movie = await _movieHubRepository.GetMovieAsync(movieId, includeCinemas);
        if (movie == null) return NotFound();

        var movieDto = _mapper.Map<MovieDto>(movie);
        if (includeCinemas)
        {
            movieDto.Cinemas = _mapper.Map<ICollection<CinemaDto>>(movie.MovieCinemas);
            
            // Feature 3, add additional cinemas from a cinema provider
            var additionalCinemas = await _cinemaProvider.GetCinemasForMovieAsync(movieDto.PrincessTheatreMovieId);
            ((List<CinemaDto>)movieDto.Cinemas).AddRange(additionalCinemas);
        }

        return Ok(movieDto);
    }
}