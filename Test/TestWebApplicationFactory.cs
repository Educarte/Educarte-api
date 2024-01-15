using Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Test;

internal class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    public TestWebApplicationFactory() { }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("Test");
        builder.ConfigureServices(_services =>
        {
            //Override DbContext
            _services.Remove(_services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApiDbContext>)));
            _services.AddDbContext<ApiDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });
        });
    }

}