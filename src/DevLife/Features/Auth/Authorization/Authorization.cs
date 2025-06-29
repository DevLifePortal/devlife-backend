using System.IdentityModel.Tokens.Jwt;
using DevLife.Infrastructure.Database.Postgres.Repository;
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
            CancellationToken cancellationToken)
        {
            
            var isUserExist = await userRepository
                .Where(u => u.Username == request.Username)
                .AnyAsync(cancellationToken);

            if (!isUserExist)
                return Results.NotFound("User not found");
            
            var token = jwtService.GenerateToken(request.Username);
            
            return Results.Ok(new
            {
                token = token,
            });
        }
    }
}