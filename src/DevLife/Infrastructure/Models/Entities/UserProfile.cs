using DevLife.Infrastructure.Models.Abstractions;
using DevLife.Infrastructure.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevLife.Infrastructure.Models.Entities;

public class UserProfile : IMongoEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public string Username { get; set; } //fg
    public Gender Gender { get; set; }
    public GenderPreference Preference { get; set; }
    public string Bio { get; set; }
    public List<string> TechStack { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public PersonalityType? PersonalityType { get; set; }

    public static UserProfile Create(string userName, Gender gender, GenderPreference preference, string bio,
        List<string> techStack)
    {
        return new UserProfile
        {
            Id = Guid.NewGuid(),
            Username = userName,
            Gender = gender,
            Preference = preference,
            Bio = bio,
            TechStack = techStack,
            CreatedAt =  DateTimeOffset.UtcNow
            
        };
    }
}