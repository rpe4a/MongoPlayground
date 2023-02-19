using System.Collections.Immutable;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoPlayground.Models;

namespace MyApp.Infrastructure;

public class MongoDatabaseInitializer
{
    private readonly MongoContext _context;
    private readonly MongoDbOptions _options;

    public MongoDatabaseInitializer(MongoContext context, IOptions<MongoDbOptions> options)
    {
        _context = context;
        _options = options.Value;
    }

    public async Task InitializeDatabaseAsync(CancellationToken token = default)
    {
        await Task.WhenAll(
            TryCreateCollection(_options.Collections.PeopleCollectionName, token),
            TryCreateCollection(_options.Collections.ZipCodesCollectionName, token),
            TryCreateCollection(_options.Collections.RestaurantsCollectionName, token));

        await InitializeIndexesAsync(token);
    }

    private async Task TryCreateCollection(string collectionName, CancellationToken token)
    {
        var collections = await _context.Database.ListCollectionNamesAsync(cancellationToken: token);
        while (await collections.MoveNextAsync(token))
        {
            if (!collections.Current.Contains(collectionName))
                _context.Database.CreateCollectionAsync(collectionName, cancellationToken: token);
        }
    }

    private async Task InitializeIndexesAsync(CancellationToken token)
    {
        var indexManager = _context.ZipCodes.Indexes;

        var stateIndex = Builders<ZipCode>.IndexKeys.Ascending(x => x.State);
        await indexManager.CreateOneAsync(
            new CreateIndexModel<ZipCode>(stateIndex, new CreateIndexOptions() {Name = "state_asc", Background = true}),
            cancellationToken: token);
    }
}