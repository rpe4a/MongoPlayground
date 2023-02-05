using Microsoft.Extensions.Options;

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

    public async Task InitializeDatabase(CancellationToken token = default)
    {
        await Task.WhenAll(
            _context.Database.CreateCollectionAsync(_options.Collections.PeopleCollectionName,
                cancellationToken: token),
            _context.Database.CreateCollectionAsync(_options.Collections.ZipCodesCollectionName,
                cancellationToken: token),
            _context.Database.CreateCollectionAsync(_options.Collections.RestaurantsCollectionName,
                cancellationToken: token));
    }
}