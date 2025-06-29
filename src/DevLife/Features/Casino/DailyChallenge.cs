using System.Security.Claims;
using DevLife.Infrastructure.Database.Mongo.Repository;
using DevLife.Infrastructure.Database.Postgres.Repository;
using DevLife.Infrastructure.Models.Entities;
using DevLife.Infrastructure.Services.OpenAI;
using Microsoft.EntityFrameworkCore;

namespace DevLife.Features.Casino;

public class DailyChallenge
{
    private record DailyChallengeResponse(Guid Id, string LeftSnippet, string RightSnippet, decimal Prise);
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("casino/daily-challenge", Handler)
                .RequireAuthorization()
                .WithTags("Code Casino");
        }

        private static async Task<IResult> Handler(  
            ClaimsPrincipal userPrincipal,
            IPostgresRepository<User> userRepository,
            IMongoRepository<GameSession> gameSessionRepository,
            ChatGptService chatGptService,
            CancellationToken cancellationToken)
        {
            var userNameClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userNameClaim)) 
                return Results.Unauthorized();
            
            var user = await userRepository
                .Where(u => u.Username == userNameClaim)
                .FirstOrDefaultAsync(cancellationToken);
            if (user is null)
                return Results.NotFound();

            var dailySession = GameSession.Create(
                user.Id,
                await chatGptService.GetIncorrectSnippet(user.TechStack, user.ExperienceLevel),
                await chatGptService.GetCorrectSnippet(),
                300m);

            await gameSessionRepository.AddAsync(dailySession, cancellationToken);

            return Results.Ok(new DailyChallengeResponse(
                dailySession.Id,
                dailySession.LeftSnippet,
                dailySession.RightSnippet,
                dailySession.Bet));
        }
    }
}