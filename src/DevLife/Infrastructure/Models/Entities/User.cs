using DevLife.Infrastructure.Models.Enums;
using DevLife.Infrastructure.Models.ValueObjects;

namespace DevLife.Infrastructure.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public DateOnly Birthday { get; set; }
    public ZodiacSign ZodiacSign { get; set; }
    public TechStack TechStack { get; set; }
    public ExperienceLevel ExperienceLevel { get; set; }
    public decimal Balance { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public User() { }

    public static User Create(string userName, string fullName, DateOnly birthDay, ExperienceLevel experienceLevel,
        TechStack stackStrings)
    {
        return new User
        {
            Username = userName,
            FullName = fullName,
            Birthday = birthDay,
            ZodiacSign = ZodiacSign.FromDate(birthDay),
            TechStack = stackStrings,
            Balance = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            ExperienceLevel = experienceLevel
        };
    }
}