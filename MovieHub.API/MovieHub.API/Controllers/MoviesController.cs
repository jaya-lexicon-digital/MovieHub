using Microsoft.AspNetCore.Mvc;
using MovieHub.API.Models;

namespace MovieHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(ILogger<MoviesController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<MovieDto> GetMovies(string? title, string? genre, string? searchQuery, int pageNumber = 1,
        int pageSize = 10)
    {
        var movie = new MovieDto();
        _logger.LogInformation("JAYA TODO");
        return Ok(movie);
    }
}