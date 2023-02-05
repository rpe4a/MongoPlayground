using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MyApp.Infrastructure;

namespace MongoPlayground;
// Note: actual namespace depends on the project name.

internal static class Program
{
    private static void Main(string[] args)
    {
        var configuration = GetConfiguration();
        var serviceProvider = GetServiceProvider(configuration);
        var mongoClient = serviceProvider.GetService<MongoContext>();
        var mongoDatabaseInitializer = serviceProvider.GetService<MongoDatabaseInitializer>();

        mongoDatabaseInitializer.InitializeDatabase();
        mongoClient.TestConnection();
    }

    private static ServiceProvider GetServiceProvider(IConfigurationRoot configuration)
    {
        if (configuration == null) 
            throw new ArgumentNullException(nameof(configuration));
        
        var serviceProvider = new ServiceCollection()
            .Configure<MongoDbOptions>(configuration.GetSection(MongoDbOptions.Key))
            .AddTransient<MongoContext>()
            .AddSingleton<MongoDatabaseInitializer>()
            .AddSingleton<IMongoClient>(_ =>
                new MongoClient(configuration.GetSection(MongoDbOptions.Key).Get<MongoDbOptions>()?.ConnectionString))
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