using MongoDB.Bson.Serialization.Attributes;

namespace MongoPlayground;

public class ZipCodePopulation
{
    [BsonId] [BsonElement("_id")] public string State { get; set; }
    [BsonElement("Population")] public string Population { get; set; }
}