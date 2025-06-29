using DevLife.Infrastructure.Models.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevLife.Infrastructure.Models.Entities;

public class ProfileInteraction : IMongoEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public Guid SourceProfileId { get; set; } 
    public Guid TargetProfileId { get; set; }  
    public DateTimeOffset Timestamp { get; set; }

    public static ProfileInteraction Create(Guid sourceProfileId, Guid targetProfileId)
    {
        return new ProfileInteraction
        {
            Id = Guid.NewGuid(),
            SourceProfileId = sourceProfileId,
            TargetProfileId = targetProfileId,
            Timestamp = DateTimeOffset.UtcNow
        };
    }
}