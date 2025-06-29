using DevLife.Infrastructure.Models.Abstractions;
using DevLife.Infrastructure.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevLife.Infrastructure.Models.Entities;

public class GameSession : IMongoEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public string WrongSnippet { get; set; }
    public string CorrectSnippet { get; set; }
    public CodeSlot CorrectCodeSlot { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string LeftSnippet { get; set; }
    public string RightSnippet { get; set; }
    
    public decimal Bet { get; set; }
    public GameSession() { }

    public static GameSession Create(int userId, string wrongSnippet, string correctSnippet, decimal bet)
    {
        var correctSlot = Random.Shared.Next(0, 2) == 0 ? CodeSlot.Left : CodeSlot.Right;
        return new GameSession
        {
            CorrectSnippet = correctSnippet,
            CorrectCodeSlot = correctSlot,
            UserId = userId,
            WrongSnippet = wrongSnippet,
            LeftSnippet = correctSlot == CodeSlot.Left ? correctSnippet : wrongSnippet,
            RightSnippet = correctSlot == CodeSlot.Right ? correctSnippet : wrongSnippet,
            Bet = bet,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }
}