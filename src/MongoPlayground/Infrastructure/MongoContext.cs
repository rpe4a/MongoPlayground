using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MyApp.Infrastructure;

public class MongoContext
{
    public MongoContext(IMongoClient client, IOptions<MongoDbOptions> options)
    {
        Client = client;
        Database = Client.GetDatabase(options.Value.Database);
    }

    private IMongoClient Client { get; }
    public IMongoDatabase Database { get; }

    public void TestConnection()
    {
        var dbsCursor = Client.ListDatabases();
        var dbsList = dbsCursor.ToList();
        foreach (var db in dbsList) Console.WriteLine(db);
    }
}