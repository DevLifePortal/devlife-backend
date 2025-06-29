namespace DevLife.Infrastructure.Services.Jwt;

public interface IJwtService
{
    public string GenerateToken(string userName);
}