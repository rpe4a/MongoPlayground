using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoPlayground.Models;

public class Person
{
    public string Id { get; set; }

    public string Name { get; set; }
    public int Age { get; set; }

    [BsonDateTimeOptions(DateOnly = true, Kind = DateTimeKind.Utc, Representation = BsonType.DateTime)]
    public DateTime DateBirth { get; set; }

    public List<string> Hobbies { get; set; }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Age)}: {Age}, {nameof(DateBirth)}: {DateBirth}, {nameof(Hobbies)}: {Hobbies}";
    }
}