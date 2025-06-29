using System.Security.Claims;
using System.Text.Json;
using DevLife.Infrastructure.Database.Postgres.Repository;
using DevLife.Infrastructure.Models.Entities;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using AddFavoriteCommand = DevLife.Features.GenerateExcuses.AddToFavorite.AddFavoriteCommand;

namespace DevLife.Features.GenerateExcuses;

public class GetFavorites
{
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("excuse/favorites", Handler)
                .RequireAuthorization()
                .WithTags("Excuse Generator");
        }

        private static async Task<IResult> Handler(
            ClaimsPrincipal userPrincipal,
            IConnectionMultiplexer redis,
            IPostgresRepository<User> userRepository,
            CancellationToken cancellationToken)
        {
            var userName  = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userName))
                return Results.Unauthorized();

            var user = await userRepository
                .Where(u => u.Username == userName)
                .FirstOrDefaultAsync(cancellationToken);
            
            var db = redis.GetDatabase();
            var key = $"favorites:{user.Username}";

            var values = await db.ListRangeAsync(key);

            var favorites = values
                .Select(v => JsonSerializer.Deserialize<AddFavoriteCommand>(v!))
                .Where(v => v is not null)
                .ToList();

            return Results.Ok(favorites);
        }
    }
}