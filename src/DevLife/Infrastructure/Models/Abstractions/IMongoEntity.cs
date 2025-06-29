using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevLife.Infrastructure.Models.Abstractions;

public interface IMongoEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    Guid Id { get; set; }
}