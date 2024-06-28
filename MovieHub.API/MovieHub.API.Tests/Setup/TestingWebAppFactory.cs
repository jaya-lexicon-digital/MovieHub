using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieHub.API.DbContexts;

namespace MovieHub.API.Tests.Setup;

public class TestingWebAppFactory<T> : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace original MovieHubDbContext with in-memory DB
            var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MovieHubDbContext>));
            if (dbContext != null) services.Remove(dbContext);
            var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
            services.AddDbContext<MovieHubDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryMovieHubTest");
                options.UseInternalServiceProvider(serviceProvider);
            });
            
            // Create ServiceProvider and scope of the service, so that the new service (EF Core in-memory database)
            // can be provided to other classes through Dependency Injection. 
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<MovieHubDbContext>())
                {
                    try
                    {
                        appContext.Database.EnsureCreated();
                        SampleData.PopulateDb(appContext);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
        });
    }
}