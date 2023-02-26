using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoPlayground.Models;

public static class PersonClassMapper
{
    public static void Register()
    {
        BsonClassMap.RegisterClassMap<Person>(classMapInitializer =>
        {
            classMapInitializer.AutoMap();
            classMapInitializer.MapIdMember(p => p.Id);
            classMapInitializer.MapMember(p => p.Age);
            classMapInitializer.MapMember(p => p.DateBirth).SetSerializer(new DateTimeSerializer(dateOnly: true));
            classMapInitializer.MapMember(p => p.Hobbies).SetElementName("favorite_hobbies");

            classMapInitializer.SetIgnoreExtraElements(true);
            classMapInitializer.SetDiscriminator(nameof(Person));
        });
    }
}