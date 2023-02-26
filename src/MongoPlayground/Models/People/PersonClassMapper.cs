using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoPlayground.Models;

public static class PersonClassMapper
{
    public static void Register()
    {
        BsonClassMap.RegisterClassMap<Person>(classMapInitializer =>
        {
            classMapInitializer.AutoMap();
            classMapInitializer.MapIdMember(p => p.Id)
                .SetSerializer(new StringSerializer(BsonType.ObjectId))
                .SetIdGenerator(new StringObjectIdGenerator());
            classMapInitializer.MapMember(p => p.Age);
            classMapInitializer.MapMember(p => p.Hobbies).SetElementName("favorite_hobbies");

            classMapInitializer.SetIgnoreExtraElements(true);
            classMapInitializer.SetDiscriminator(nameof(Person));
        });
    }
}