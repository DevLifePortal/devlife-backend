using System.Security.Claims;
using DevLife.Infrastructure.Database.Redis.RefreshToken;
using DevLife.Infrastructure.Services.Jwt;

namespace DevLife.Features.Auth.RefreshToken;

public class RefreshToken
{
    public record RefreshCommand(string RefreshToken);
    
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("auth/refresh-token", Handler)
                .RequireAuthorization()
                .WithTags("Auth");
        }

        private static async Task<IResult> Handler( 
            RefreshCommand command,
            ClaimsPrincipal userPrincipal,
            IRefreshTokenStorage refreshTokenStorage,
            IJwtService jwtService,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(command.RefreshToken))
                return Results.BadRequest();
            
            var userName = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userName))
                return Results.Unauthorized();
            
            var actuallyRefreshToken = await refreshTokenStorage.GetAsync(userName, cancellationToken);
            if (actuallyRefreshToken is null )
                return Results.Unauthorized();
            
            var newAccessToken = jwtService.GenerateToken(userName);
            var newRefreshToken = Guid.NewGuid().ToString();
            
            await refreshTokenStorage.SetAsync(
                userName, newRefreshToken, 
                TimeSpan.FromDays(7), cancellationToken);
            
            return Results.Ok(new RefreshTokenResponse(newAccessToken, newRefreshToken));
        }
    }
    private record RefreshTokenResponse(string AccessToken, string RefreshToken);
}