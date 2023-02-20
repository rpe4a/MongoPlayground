using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoPlayground.Models;
using MyApp.Infrastructure;

namespace MongoPlayground;
// Note: actual namespace depends on the project name.

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var configuration = GetConfiguration();
        var serviceProvider = GetServiceProvider(configuration);
        var mongoContext = serviceProvider.GetRequiredService<MongoContext>();
        var mongoDatabaseInitializer = serviceProvider.GetRequiredService<MongoDatabaseInitializer>();

        await mongoDatabaseInitializer.InitializeDatabaseAsync();

        //CRUD 
        // await InsertRestaurant(mongoContext);
        // await ReplaceRestaurant(mongoContext);
        // await FindAndReplaceRestaurant(mongoContext);
        // await UpdateRestaurant(mongoContext);
        // await DeleteRestaurant(mongoContext);
        // await BulkRestaurants(mongoContext);
        // await QueryToRestaurant(mongoContext);
        // await QueryToZipCode(mongoContext);

        //Aggregation
        // await LooselyTypeAggregationZipCodeOperations(mongoContext);
        // await StrongTypeAggregationZipCodeOperations(mongoContext);

        //Indexes
        await WorkingWithIndexes(mongoContext);
    }

    private static async Task WorkingWithIndexes(MongoContext mongoContext)
    {
        var indexZipCodeManager = mongoContext.ZipCodes.Indexes;

        var indexes = await indexZipCodeManager.ListAsync();

        while (await indexes.MoveNextAsync())
        {
            var currentIndex = indexes.Current;
            foreach (var doc in currentIndex)
            {
                var docNames = doc.Names;
                foreach (string name in docNames)
                {
                    var value = doc.GetValue(name);
                    Console.WriteLine(string.Concat(name, ": ", value));
                }
            }
        }
    }

    private static async Task StrongTypeAggregationZipCodeOperations(MongoContext mongoContext)
    {
        // db.zipcodes.aggregate([
        // {$group: { "_id" : "$state", "population" : {$sum : "$pop"}}}, 
        // {$match : {population : {$gte : 5000000}}}, 
        // {$sort: {"_id" : 1}}, 
        // {$limit : 5}])

        var result = mongoContext.ZipCodes.Aggregate()
            .Group(
                key => key.State,
                value => new {State = value.Key, Population = value.Sum(key => key.Population)})
            .Match(x => x.Population >= 500000)
            .SortBy(x => x.State)
            .Limit(5)
            .As<ZipCodePopulation>()
            .ToList();

        foreach (var item in result)
        {
            Console.WriteLine($"{item.State}: {item.Population}");
        }
    }

    private static async Task LooselyTypeAggregationZipCodeOperations(MongoContext mongoContext)
    {
        // db.zipcodes.aggregate([
        // {$group: { "_id" : "$state", "population" : {$sum : "$pop"}}}, 
        // {$match : {population : {$gte : 5000000}}}, 
        // {$sort: {"_id" : 1}}, 
        // {$limit : 5}])
        var groupPipeline = new BsonDocument
        {
            {
                "$group",
                new BsonDocument {{"_id", "$state"}}
                    .Add("population", new BsonDocument {{"$sum", "$pop"}})
            }
        };

        var matchPipeline = new BsonDocument()
            {{"$match", new BsonDocument("population", new BsonDocument("$gte", 5000000))}};

        var sortPipeline = new BsonDocument("$sort", new BsonDocument("_id", 1));
        var limitPipeline = new BsonDocument("$limit", 5);

        var resultList = mongoContext.ZipCodes.Aggregate()
            .AppendStage<BsonDocument>(groupPipeline)
            .AppendStage<BsonDocument>(matchPipeline)
            .AppendStage<BsonDocument>(sortPipeline)
            .AppendStage<BsonDocument>(limitPipeline)
            .ToList();

        foreach (var item in resultList)
        {
            Console.WriteLine($"{item}");
        }
    }

    private static async Task BulkRestaurants(MongoContext mongoContext)
    {
        Restaurant newThaiRestaurant = new Restaurant
        {
            Address = new RestaurantAddress()
            {
                BuildingNr = "150",
                Coordinates = new double[] {22.82, 99.12},
                Street = "Old Street",
                ZipCode = 876654
            },
            Borough = "Somewhere in Thailand",
            Cuisine = "Thai",
            Grades = new List<RestaurantGrade>()
            {
                new RestaurantGrade() {Grade = "A", InsertedUtc = DateTime.UtcNow, Score = "7"},
                new RestaurantGrade() {Grade = "B", InsertedUtc = DateTime.UtcNow, Score = "4"},
                new RestaurantGrade() {Grade = "B", InsertedUtc = DateTime.UtcNow, Score = "10"},
                new RestaurantGrade() {Grade = "B", InsertedUtc = DateTime.UtcNow, Score = "4"}
            },
            Id = 463456435,
            Name = "FavThai"
        };

        var updateRestaurant = Builders<Restaurant>.Update.Set(x => x.Borough, "SomeBorough");

        BulkWriteResult bulkWriteResult = await mongoContext.Restaurants.BulkWriteAsync(new WriteModel<Restaurant>[]
        {
            new InsertOneModel<Restaurant>(newThaiRestaurant),
            new UpdateOneModel<Restaurant>(Builders<Restaurant>.Filter.Eq(r => r.Name, "RandomThai"), updateRestaurant),
            new DeleteOneModel<Restaurant>(Builders<Restaurant>.Filter.Eq(r => r.Name, "PakistaniKing")),
        }, new BulkWriteOptions() {IsOrdered = true});

        Console.WriteLine(string.Concat("Deleted count: ", bulkWriteResult.DeletedCount));
        Console.WriteLine(string.Concat("Inserted count: ", bulkWriteResult.InsertedCount));
        Console.WriteLine(string.Concat("Acknowledged: ", bulkWriteResult.IsAcknowledged));
        Console.WriteLine(string.Concat("Matched count: ", bulkWriteResult.MatchedCount));
        Console.WriteLine(string.Concat("Modified count: ", bulkWriteResult.ModifiedCount));
        Console.WriteLine(string.Concat("Request count: ", bulkWriteResult.RequestCount));
        Console.WriteLine(string.Concat("Upsert count: ", bulkWriteResult.Upserts));
    }

    private static async Task DeleteRestaurant(MongoContext mongoContext)
    {
        var result = await mongoContext.Restaurants
            .DeleteOneAsync(x => x.Name == "BrandNewMexicanKing");
    }

    private static async Task UpdateRestaurant(MongoContext mongoContext)
    {
        var updateDefinition = Builders<Restaurant>.Update
            .Set(r => r.Borough, "New Borough")
            .Push(r => r.Grades, new RestaurantGrade() {Grade = "A", InsertedUtc = DateTime.UtcNow, Score = "6"});

        var result = await mongoContext.Restaurants.UpdateOneAsync(x => x.Name == "BrandNewMexicanKing",
            updateDefinition, new UpdateOptions() {IsUpsert = true});
    }

    private static async Task FindAndReplaceRestaurant(MongoContext mongoContext)
    {
        Restaurant mexicanReplacement = new Restaurant
        {
            Address = new RestaurantAddress()
            {
                BuildingNr = "4/D",
                Coordinates = new double[] {24.68, -100.9},
                Street = "New Mexico Street",
                ZipCode = 768324865
            },
            Borough = "In the middle of Mexico",
            Cuisine = "Mexican",
            Grades = new List<RestaurantGrade>()
            {
                new RestaurantGrade() {Grade = "B", InsertedUtc = DateTime.UtcNow, Score = "10"},
                new RestaurantGrade() {Grade = "B", InsertedUtc = DateTime.UtcNow, Score = "4"}
            },
            Id = 457656745,
            Name = "BrandNewMexicanKing",
        };

        var result = await mongoContext.Restaurants
            .FindOneAndReplaceAsync<Restaurant>(x => x.Name == "BrandNewMexicanKing",
                mexicanReplacement,
                new FindOneAndReplaceOptions<Restaurant>()
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After,
                    Sort = Builders<Restaurant>.Sort.Ascending(r => r.Name)
                });
    }

    private static async Task ReplaceRestaurant(MongoContext mongoContext)
    {
        Restaurant mexicanReplacement = new Restaurant
        {
            Address = new RestaurantAddress()
            {
                BuildingNr = "3/D",
                Coordinates = new double[] {24.68, -100.9},
                Street = "Mexico Street",
                ZipCode = 768324865
            },
            Borough = "Somewhere in Mexico",
            Cuisine = "Mexican",
            Grades = new List<RestaurantGrade>()
            {
                new RestaurantGrade() {Grade = "B", InsertedUtc = DateTime.UtcNow, Score = "10"},
                new RestaurantGrade() {Grade = "B", InsertedUtc = DateTime.UtcNow, Score = "4"}
            },
            Id = 457656745,
            Name = "NewMexicanKing",
            MongoDbId = ObjectId.Parse("63e796f0160c4bc3ea13fd60")
        };

        var result = await mongoContext.Restaurants
            .ReplaceOneAsync(x => x.Name == "RandomThai", mexicanReplacement, new UpdateOptions() {IsUpsert = true});
    }

    private static async Task InsertRestaurant(MongoContext mongoContext)
    {
        Restaurant newRestaurant = new Restaurant
        {
            Address = new RestaurantAddress()
            {
                BuildingNr = "120",
                Coordinates = new double[] {22.82, 99.12},
                Street = "Whatever",
                ZipCode = 123456
            },
            Borough = "Somewhere in Thailand",
            Cuisine = "Thai",
            Grades = new List<RestaurantGrade>()
            {
                new RestaurantGrade() {Grade = "A", InsertedUtc = DateTime.UtcNow, Score = "7"},
                new RestaurantGrade() {Grade = "B", InsertedUtc = DateTime.UtcNow, Score = "4"}
            },
            Id = 883738291,
            Name = "RandomThai"
        };

        await mongoContext.Restaurants.InsertOneAsync(newRestaurant);
    }

    private static async Task QueryToZipCode(MongoContext mongoContext)
    {
        var zipcode = await mongoContext.ZipCodes
            .Find(x => x.State == "MA")
            .SortBy(x => x.State)
            .ThenBy(x => x.City)
            .FirstAsync();

        var combineSorting = Builders<ZipCode>.Sort
            .Ascending(x => x.State)
            .Descending(x => x.City);

        var zipcodes = await mongoContext.ZipCodes
            .Find(x => true)
            .Limit(3)
            .Sort(combineSorting)
            .ToListAsync();

        Console.WriteLine(string.Join(", ", zipcodes));
    }

    private static async Task QueryToRestaurant(MongoContext mongoContext)
    {
        var filterBorough = Builders<Restaurant>.Filter
            .Eq(x => x.Borough, "Brooklyn");

        var filterCuisine = Builders<Restaurant>.Filter
            .Eq(x => x.Cuisine, "Delicatessen");

        var filterAnd = Builders<Restaurant>.Filter
            .And(filterBorough, filterCuisine);

        var filterNestedElement = Builders<Restaurant>.Filter
            .ElemMatch(x => x.Grades, x => x.Grade == "A");

        var sortByName = Builders<Restaurant>.Sort
            .Ascending(x => x.Name);

        var restaurants =
            await mongoContext.Restaurants
                .Find(filterAnd)
                .Limit(5)
                .Sort(sortByName)
                .ToListAsync();

        var restaurant = mongoContext.Restaurants
            .FindSync(filterNestedElement)
            .FirstOrDefault();

        Console.WriteLine(restaurant);
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
            {
                // MongoDb settings
                var settings = MongoClientSettings.FromConnectionString(
                    configuration.GetSection(MongoDbOptions.Key).Get<MongoDbOptions>()?.ConnectionString);

                settings.ReadConcern = new ReadConcern(ReadConcernLevel.Majority);
                settings.ReadPreference = ReadPreference.Secondary;
                settings.WriteConcern = WriteConcern.WMajority;

                return new MongoClient(settings);
            })
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