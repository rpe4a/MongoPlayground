using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoPlayground.Models;

[BsonIgnoreExtraElements]
public class Restaurant
{
    [BsonId]
    [BsonElement(elementName: "_id")]
    [BsonIgnoreIfDefault]
    public ObjectId MongoDbId { get; set; }

    [BsonElement(elementName: "address")] public RestaurantAddress Address { get; set; }

    [BsonElement(elementName: "borough")] public string Borough { get; set; }

    [BsonElement(elementName: "cuisine")] public string Cuisine { get; set; }

    [BsonElement(elementName: "grades")] public IEnumerable<RestaurantGrade> Grades { get; set; }

    [BsonElement(elementName: "name")] public string Name { get; set; }

    [BsonElement(elementName: "restaurant_id")]
    [BsonRepresentation(BsonType.String)]
    public int Id { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}