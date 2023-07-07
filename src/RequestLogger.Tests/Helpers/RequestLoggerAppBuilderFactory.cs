using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace RequestLogger.Tests.Helpers;

public class RequestLoggerAppBuilderFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    //private readonly Dictionary<string, string> _configuration = new();
    
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var projectDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(projectDir, "appsettings.Testing.json");

        builder
            .ConfigureAppConfiguration((context, conf) =>
            {
                conf.AddJsonFile(configPath);
                //conf.AddInMemoryCollection(_configuration);
            })
            .UseEnvironment("Testing"); 
    }
}