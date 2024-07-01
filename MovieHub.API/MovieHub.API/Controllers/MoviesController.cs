using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MovieHub.API.Models;
using MovieHub.API.Services;

namespace MovieHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieHubRepository _movieHubRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<MoviesController> _logger;
    private const int MaxMoviesPageSize = 100;

    public MoviesController(IMovieHubRepository movieHubRepository, IMapper mapper, ILogger<MoviesController> logger)
    {
        _movieHubRepository = movieHubRepository ?? throw new ArgumentNullException(nameof(movieHubRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerator<MovieDto>>> GetMovies(string? title, string? genre,
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
        
        return Ok(_mapper.Map<IEnumerable<MovieWithoutCinemasDto>>(movieEntities));
    }

    [HttpGet("{movieId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<MovieDto>> GetMovie(int movieId, bool includeCinemas = true)
    {
        var movie = await _movieHubRepository.GetMovieAsync(movieId, includeCinemas);
        if (movie == null)
        {
            return NotFound();
        }

        var movieDto = _mapper.Map<MovieDto>(movie);
        if (includeCinemas)
        {
            movieDto.Cinemas = _mapper.Map<ICollection<CinemaDto>>(movie.MovieCinemas);
        }

        return Ok(movieDto);
    }
}