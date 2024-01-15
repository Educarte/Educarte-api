using Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Test;

public class BaseTest : IClassFixture<ServerFixture>
{
    protected readonly ApiDbContext db;
    protected readonly HttpClient client;

    public BaseTest(ServerFixture serverFixture)
    {
        db = serverFixture.ServiceProvider.GetRequiredService<ApiDbContext>();
        client = serverFixture.Client;
    }
}