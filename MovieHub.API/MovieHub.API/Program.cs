using Microsoft.EntityFrameworkCore;
using MovieHub.API.DbContexts;
using MovieHub.API.Services;
using MovieHub.API.Services.PrincessTheatre;
using MovieHub.API.Services.PrincessTheatre.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MovieHubDbContext>(dbContextOptions =>
    dbContextOptions.UseSqlite(builder.Configuration.GetConnectionString("MovieHubDBConnectionString")));

builder.Services.Configure<PrincessTheatreOptions>(
    builder.Configuration.GetSection(PrincessTheatreOptions.PrincessTheatre)
);

builder.Services.AddHttpClient<ICinemaService, PrincessTheatreCinemaService>()
    .AddStandardResilienceHandler();
// For defaults of StandardResilienceHandler refer to:
// https://learn.microsoft.com/en-us/dotnet/core/resilience/http-resilience?tabs=dotnet-cli#standard-resilience-handler-defaults

builder.Services.AddTransient<IMovieHubService, MovieHubService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Note: the below line, allows us to later create a Test Server via WebApplicationFactory<Program>
public partial class Program { }