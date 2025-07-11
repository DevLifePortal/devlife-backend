namespace DevLife.Infrastructure.Services.Jwt;

public class JwtConfiguration
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpiresAtMinutes { get; set; }
}