using MongoDB.Bson.Serialization.Attributes;

namespace MongoPlayground.Models;

public class Person
{
    public string Id { get; set; }

    public string Name { get; set; }
    public int Age { get; set; }
    [BsonDateTimeOptions()]
    public DateTime DateBirth { get; set; }
    public List<string> Hobbies { get; set; }
}