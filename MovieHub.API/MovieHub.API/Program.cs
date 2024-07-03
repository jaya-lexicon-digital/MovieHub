using Microsoft.EntityFrameworkCore;
using MovieHub.API.DbContexts;
using MovieHub.API.Services;
using MovieHub.API.Services.PrincessTheatre;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MovieHubDbContext>(dbContextOptions =>
    dbContextOptions.UseSqlite(builder.Configuration["ConnectionStrings:MovieHubDBConnectionString"]));

builder.Services.AddScoped<IMovieHubRepository, MovieHubRepository>();
builder.Services.AddScoped<ResilientHttpClient>();
builder.Services.AddScoped<ICinemaProvider, PrincessTheatreCinemaProvider>();

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