using System.Text.Json;
using System.Xml;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoPlayground.Models;

public class ZipCode
{
    [BsonId]
    [BsonElement(elementName: "_id")]
    public string Id { get; set; }

    [BsonElement(elementName: "city")] public string City { get; set; }
    [BsonElement(elementName: "loc")] public double[] Coordinates { get; set; }
    [BsonElement(elementName: "pop")] public int Population { get; set; }
    [BsonElement(elementName: "state")] public string State { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}