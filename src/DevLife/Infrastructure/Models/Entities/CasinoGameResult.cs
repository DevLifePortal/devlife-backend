using DevLife.Infrastructure.Models.Enums;
using Microsoft.AspNetCore.SignalR;

namespace DevLife.Infrastructure.Models.Entities;

public class CasinoGameResult
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public CodeSlot UserAnswer { get; set; }
    public CodeSlot CorrectAnswer { get; set; }
    public bool IsCorrect { get; set; }
    public decimal Bet { get; set; }
    public decimal Points { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public CasinoGameResult() { }

    public static CasinoGameResult Create(int userId, CodeSlot userAnswer, CodeSlot correctAnswer, bool isCorrect,
        decimal bet, decimal points)
    {
        return new CasinoGameResult
        {
            UserId = userId,
            UserAnswer = userAnswer,
            CorrectAnswer = correctAnswer,
            IsCorrect = isCorrect,
            Bet = bet,
            Points = points,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}