using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoPlayground.Models;

[BsonIgnoreExtraElements]
public class RestaurantAddress
{
    [BsonElement(elementName: "building")]
    public string BuildingNr { get; set; }
    
    [BsonElement(elementName: "coord")]
    public double[] Coordinates { get; set; }
    
    [BsonElement(elementName: "street")]
    public string Street { get; set; }
    
    [BsonElement(elementName:"zipcode")]
    [BsonRepresentation(BsonType.String)]
    public int ZipCode { get; set; }

    public override string ToString()
    {
        return $"{nameof(BuildingNr)}: {BuildingNr}, {nameof(Coordinates)}: {Coordinates}, {nameof(Street)}: {Street}, {nameof(ZipCode)}: {ZipCode}";
    }
}