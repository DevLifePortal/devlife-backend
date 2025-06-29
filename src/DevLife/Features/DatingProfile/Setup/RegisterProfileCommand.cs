using DevLife.Infrastructure.Models.Enums;

namespace DevLife.Features.DatingProfile.Setup;

public record RegisterProfileCommand(
    string Username,
    Gender Gender,
    GenderPreference Preference,
    string Bio,
    List<string> TechStack);