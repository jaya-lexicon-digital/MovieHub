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
    private readonly IMovieHubService _movieHubService;
    private readonly ICinemaService _cinemaService;
    private readonly IMapper _mapper;
    private readonly ILogger<MoviesController> _logger;
    private const int MaxMoviesPageSize = 100;

    public MoviesController(IMovieHubService movieHubService, ICinemaService cinemaService,
        IMapper mapper, ILogger<MoviesController> logger)
    {
        _movieHubService = movieHubService ?? throw new ArgumentNullException(nameof(movieHubService));
        _cinemaService = cinemaService ?? throw new ArgumentNullException(nameof(cinemaService));
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
            _logger.LogInformation("Warning pageSize of: '{pageSize}' exceeds the max pageSize of: "
                                   + "'{MaxMoviesPageSize}', hence changing pageSize to the maximum allowed!",
                pageSize, MaxMoviesPageSize);
            pageSize = MaxMoviesPageSize;
        }
        
        var (movieEntities, paginationMetadata) = await _movieHubService
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
        var movie = await _movieHubService.GetMovieAsync(movieId, includeCinemas);
        if (movie == null) return NotFound();

        var movieDto = _mapper.Map<MovieDto>(movie);
        if (includeCinemas)
        {
            movieDto.Cinemas = _mapper.Map<ICollection<CinemaDto>>(movie.MovieCinemas);
            
            // Feature 3, add additional cinemas from the cinema service 
            var additionalCinemas = await _cinemaService.GetCinemasForMovieAsync(movieDto.PrincessTheatreMovieId);
            ((List<CinemaDto>)movieDto.Cinemas).AddRange(additionalCinemas);
        }

        return Ok(movieDto);
    }
}