using System.Collections.Immutable;
using System.Security.Claims;
using DevLife.Infrastructure.Database.Postgres.Repository;
using DevLife.Infrastructure.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevLife.Features.Casino;

public class LeaderBoard
{
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("casino/leaderboard", Handler)
                .WithTags("Code Casino");
        }

        public static async Task<IResult> Handler(
            ClaimsPrincipal userPrincipal, 
            IPostgresRepository<CasinoGameResult> gameResultRepository,
            IPostgresRepository<User> userRepository,
            CancellationToken cancellationToken)
        {
            
            var userNameClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userNameClaim)) 
                return Results.Unauthorized();
            
            
            var topWinners = await gameResultRepository
                .AsQueryable()
                .Where(x => x.IsCorrect)
                .GroupBy(x => x.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    Wins = g.Count()
                })
                .OrderByDescending(x => x.Wins)
                .Take(10)
                .ToListAsync(cancellationToken);

            var userIds = topWinners.Select(x => x.UserId).ToImmutableArray();

            var users = await userRepository
                .AsQueryable()
                .Where(x => userIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x.Username, cancellationToken);

            var leaderboard = topWinners
                .Select(x =>
                {
                    var username = users.GetValueOrDefault(x.UserId, "Unknown");
                    return new
                    {
                        x.UserId,
                        UserName = username == userNameClaim ? "you" : username,
                        x.Wins
                    };
                })
                .ToList();
            
            return Results.Ok(leaderboard);
        }
        
    }
}