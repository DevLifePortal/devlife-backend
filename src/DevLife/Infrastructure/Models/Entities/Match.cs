using DevLife.Infrastructure.Models.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace DevLife.Infrastructure.Models.Entities;

public class Match : IMongoEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    [BsonSerializer(typeof(GuidSerializer))]
    public Guid Profile1Id { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    [BsonSerializer(typeof(GuidSerializer))]
    public Guid Profile2Id { get; set; }
    public DateTime MatchedAt { get; set; } = DateTime.UtcNow;
}