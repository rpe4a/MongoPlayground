namespace MyApp.Infrastructure;

public class MongoDbOptions
{
    public const string Key = "MongoDb";

    public string ConnectionString { get; set; }
    public string Database { get; set; }

    public Collections Collections { get; set; }
}

public class Collections
{
    public string RestaurantsCollectionName { get; set; }
    public string ZipCodesCollectionName { get; set; }
    public string PeopleCollectionName { get; set; }
}