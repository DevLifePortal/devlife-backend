using System.Security.Claims;
using System.Text.Json;
using DevLife.Infrastructure.Database.Postgres.Repository;
using DevLife.Infrastructure.Models.Entities;
using StackExchange.Redis;
using AddFavoriteCommand = DevLife.Features.GenerateExcuses.AddToFavorite.AddFavoriteCommand;

namespace DevLife.Features.GenerateExcuses;

public class DeleteFavorite
{
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapDelete("excuse/favorite/{excuseId:guid}", Handler)
                .RequireAuthorization()
                .WithTags("Excuse Generator");
        }

        private static async Task<IResult> Handler(
            ClaimsPrincipal userPrincipal,
            Guid excuseId,
            IConnectionMultiplexer redis,
            IPostgresRepository<User> userRepository,
            CancellationToken cancellationToken)
        {
            var userName  = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userName))
                return Results.Unauthorized();
            
            var db = redis.GetDatabase();
            var key = $"favorites:{userName}";

            var values = await db.ListRangeAsync(key);
            if (values.Length == 0)
                return Results.NotFound("No favorites found.");

            var parsed = values
                .Select(v => JsonSerializer.Deserialize<AddFavoriteCommand>(v!))
                .Where(v => v is not null)
                .ToList();

            var updated = parsed
                .Where(e => e!.Id != excuseId)
                .ToList();

            if (updated.Count == parsed.Count)
                return Results.NotFound("Excuse not found.");

            await db.KeyDeleteAsync(key);
            
            foreach (var excuse in updated)
                await db.ListRightPushAsync(key, JsonSerializer.Serialize(excuse));

            return Results.Ok("Excuse removed from favorites.");
        }
    }
}