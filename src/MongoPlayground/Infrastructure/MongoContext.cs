using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoPlayground.Models;

namespace MyApp.Infrastructure;

public class MongoContext
{
    private readonly MongoDbOptions _options;

    public MongoContext(IMongoClient client, IOptions<MongoDbOptions> options)
    {
        _options = options.Value;
        Client = client;
        Database = Client.GetDatabase(_options.Database);
    }

    private IMongoClient Client { get; }
    public IMongoDatabase Database { get; }

    public IMongoCollection<Restaurant> Restaurants => Database.GetCollection<Restaurant>(_options.Collections.RestaurantsCollectionName);
    public IMongoCollection<ZipCode> ZipCodes => Database.GetCollection<ZipCode>(_options.Collections.ZipCodesCollectionName);
    public IMongoCollection<Person> People => Database.GetCollection<Person>(_options.Collections.PeopleCollectionName);
}