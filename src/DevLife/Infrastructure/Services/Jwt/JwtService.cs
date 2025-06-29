using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DevLife.Infrastructure.Services.Jwt;

public class JwtService : IJwtService
{
    private readonly JwtConfiguration _configuration;

    public JwtService(IOptions<JwtConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }
    
    public string GenerateToken(string userName)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userName),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_configuration.ExpiresAtMinutes);

        var token = new JwtSecurityToken(
            issuer: _configuration.Issuer,
            audience: _configuration.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);    }
}