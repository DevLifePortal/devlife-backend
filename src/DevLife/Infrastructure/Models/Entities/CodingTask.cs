using DevLife.Infrastructure.Models.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevLife.Infrastructure.Models.Entities;

public class CodingTask : IMongoEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Language { get; set; }
    public string Difficulty { get; set; }
}