using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Test;

public class ServerFixture : IDisposable
{
    private readonly TestServer _testServer;
    public IServiceProvider ServiceProvider { get; private set; }
    public HttpClient Client { get; private set; }

    public ServerFixture()
    {
        var factory = new TestWebApplicationFactory().WithWebHostBuilder(config => { });
        _testServer = factory.Server;

        Client = _testServer.CreateClient();

        var scope = _testServer.Services.GetService<IServiceScopeFactory>().CreateScope();
        ServiceProvider = scope.ServiceProvider;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}