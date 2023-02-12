using MongoDB.Bson.Serialization.Attributes;

namespace MongoPlayground.Models;

[BsonIgnoreExtraElements]
public class RestaurantGrade
{
    [BsonElement(elementName: "date")]
    public DateTime InsertedUtc { get; set; }
    [BsonElement(elementName: "grade")]
    public string Grade { get; set; }
    [BsonElement(elementName: "score")]
    public object Score { get; set; }
}