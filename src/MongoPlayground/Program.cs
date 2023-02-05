using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Infrastructure;

namespace MongoPlayground;
// Note: actual namespace depends on the project name.

internal static class Program
{
    private static void Main(string[] args)
    {
        var configuration = GetConfiguration();
        var serviceProvider = GetServiceProvider(configuration);

        var mongoConnectionString = configuration.GetConnectionString(MongoDbOptions.ConnectionString);
        var mongoDbOptions = configuration.GetSection(MongoDbOptions.Key).Get<MongoDbOptions>();
    }

    private static ServiceProvider GetServiceProvider(IConfigurationRoot configuration)
    {
        var serviceProvider = new ServiceCollection()
            .Configure<MongoDbOptions>(configuration.GetSection(MongoDbOptions.Key))
            .BuildServiceProvider();

        return serviceProvider;
    }

    private static IConfigurationRoot GetConfiguration()
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json");
        return builder.Build();
    }
}