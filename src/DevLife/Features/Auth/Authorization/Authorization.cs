using System.IdentityModel.Tokens.Jwt;
using DevLife.Infrastructure.Database.Postgres.Repository;
using DevLife.Infrastructure.Database.Redis.RefreshToken;
using DevLife.Infrastructure.Models.Entities;
using DevLife.Infrastructure.Services.Jwt;
using Microsoft.EntityFrameworkCore;

namespace DevLife.Features.Auth.Authorization;

public class Authorization
{
    private record AuthenticationRequest(string Username);
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/authentication", Handler)
                .WithTags("Auth");
        }

        private static async Task<IResult> Handler(
            AuthenticationRequest request,
            IJwtService jwtService,
            IPostgresRepository<User> userRepository,
            IRefreshTokenStorage refreshTokenStorage,
            CancellationToken cancellationToken)
        {
            var user = await userRepository
                .Where(u => u.Username == request.Username)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return Results.NotFound("User not found");
            
            var accessToken = jwtService.GenerateToken(request.Username);
            var refreshToken = Guid.NewGuid().ToString();
            
            await refreshTokenStorage.SetAsync(
                user.Username, refreshToken, 
                TimeSpan.FromDays(7), cancellationToken);
            
            return Results.Ok(new AuthorizationResponse(
                    accessToken, refreshToken
                ));
        }
    }
    private record AuthorizationResponse(string AccessToken, string RefreshToken);
}