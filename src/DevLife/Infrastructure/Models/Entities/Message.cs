using DevLife.Infrastructure.Models.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevLife.Infrastructure.Models.Entities;

public class Message : IMongoEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public Guid SenderId { get; set; }
    public string Text { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsAiGenerated { get; set; } = false;
}