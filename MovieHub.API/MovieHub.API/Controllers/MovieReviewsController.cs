using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MovieHub.API.Models;
using MovieHub.API.Services;

namespace MovieHub.API.Controllers;

[ApiController]
[Route("api/movie/{movieId}/reviews")]
public class MovieReviewsController : ControllerBase
{
    private readonly IMovieHubService _movieHubService;
    private readonly IMapper _mapper;
    private readonly ILogger<MoviesController> _logger;
    private const int MaxReviewsPageSize = 100;

    public MovieReviewsController(IMovieHubService movieHubService, IMapper mapper,
        ILogger<MoviesController> logger)
    {
        _movieHubService = movieHubService ?? throw new ArgumentNullException(nameof(movieHubService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<MovieReviewsDto>> GetMovieReviews(int movieId, int pageNumber = 1,
        int pageSize = 20)
    {
        if (!await _movieHubService.MovieExistsAsync(movieId)) return NotFound();

        if (pageSize > MaxReviewsPageSize)
        {
            _logger.LogInformation("Warning pageSize of: '{pageSize}' exceeds the max pageSize of: "
                                   + "'{MaxReviewsPageSize}', hence changing pageSize to the maximum allowed!",
                pageSize, MaxReviewsPageSize);
            pageSize = MaxReviewsPageSize;
        }

        var (movieEntities, paginationMetadata) = await _movieHubService
            .GetReviewsForMovieAsync(movieId, pageNumber, pageSize);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        var movieReviewsDto = new MovieReviewsDto()
        {
            MovieReviews = _mapper.Map<ICollection<MovieReviewDto>>(movieEntities)
        };

        return Ok(movieReviewsDto);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> CreateMovieReview(int movieId, MovieReviewForCreationDto movieReviewForCreation)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!await _movieHubService.MovieExistsAsync(movieId)) return NotFound();

        var movieReviewEntity = _mapper.Map<Entities.MovieReview>(movieReviewForCreation);
        await _movieHubService.AddMovieReviewAsync(movieId, movieReviewEntity);
        await _movieHubService.SaveChangesAsync();
        var createdMovieReviewDto = _mapper.Map<MovieReviewDto>(movieReviewEntity);

        return Created($"api/movie/{movieId}/reviews/{createdMovieReviewDto.Id}", createdMovieReviewDto);
    }

    [HttpPut("{movieReviewId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> UpdateMovieReview(int movieId, int movieReviewId,
        MovieReviewForUpdateDto movieReviewForUpdateDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!await _movieHubService.MovieExistsAsync(movieId)) return NotFound();

        var movieReviewEntity = await _movieHubService.GetReviewForMovieAsync(movieId, movieReviewId);
        if (movieReviewEntity == null) return NotFound();

        _mapper.Map(movieReviewForUpdateDto, movieReviewEntity);
        await _movieHubService.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{movieReviewId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteMovieReview(int movieId, int movieReviewId)
    {
        if (!await _movieHubService.MovieExistsAsync(movieId)) return NotFound();
        var movieReviewEntity = await _movieHubService.GetReviewForMovieAsync(movieId, movieReviewId);
        if (movieReviewEntity == null) return NotFound();

        _movieHubService.DeleteMovieReview(movieReviewEntity);
        await _movieHubService.SaveChangesAsync();

        return NoContent();
    }
}