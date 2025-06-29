using System.Security.Claims;
using System.Text.Json;
using DevLife.Infrastructure.Database.Postgres.Repository;
using DevLife.Infrastructure.Models.Entities;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace DevLife.Features.GenerateExcuses;

public class AddToFavorite
{
    public record AddFavoriteCommand(string UserName, string Text, int BelievabilityScore, Guid Id);
    
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("excuse/favorite", Handler)
                .RequireAuthorization()
                .WithTags("Excuse Generator");
        }

        private static async Task<IResult> Handler(
                AddFavoriteCommand command,
                ClaimsPrincipal userPrincipal,
                IConnectionMultiplexer redis, 
                CancellationToken cancellationToken)
        {
            var userName  = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userName))
                return Results.Unauthorized();
            
            var db = redis.GetDatabase();

            var key = $"favorites:{command.UserName}";

            var excuseJson = JsonSerializer.Serialize(command);

            await db.ListRightPushAsync(key, excuseJson);

            return Results.Ok("Excuse added to favorites.");
        }
    }
}