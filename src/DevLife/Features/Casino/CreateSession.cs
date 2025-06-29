using System.Security.Claims;
using DevLife.Infrastructure.Database.Mongo.Repository;
using DevLife.Infrastructure.Database.Postgres.Repository;
using DevLife.Infrastructure.Models.Entities;
using DevLife.Infrastructure.Models.Enums;
using DevLife.Infrastructure.Services.OpenAI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace DevLife.Features.Casino;

public class CreateSession
{
    private record CreateGameSessionCommand(decimal Bet);    
    private record CreateGameSessionResponse(Guid GameSessionId, string LeftCodeSnippet, string RightCodeSnippet);
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("casino/start-game", Handler)
                .RequireAuthorization()
                .WithTags("Code Casino");
        }

        private static async Task<IResult> Handler(
            [FromBody] CreateGameSessionCommand command,
            ClaimsPrincipal userPrincipal,
            IMongoRepository<GameSession> repository,
            ChatGptService chatGptService,
            IPostgresRepository<User> postgresRepository,
            CancellationToken cancellationToken)
        {

            var userNameClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userNameClaim))
                return Results.Unauthorized();
            
            var user = await postgresRepository
                .Where(u => u.Username == userNameClaim)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return Results.NotFound();
            if (command.Bet <= 1 || command.Bet > user.Balance)
                    return Results.BadRequest("you can't dep this count");
            
            var gameSession = GameSession.Create(
                user.Id,
                await chatGptService.GetIncorrectSnippet(user.TechStack, user.ExperienceLevel),
                await chatGptService.GetCorrectSnippet(), 
                command.Bet);
            
            await repository.AddAsync(gameSession,cancellationToken);
            
            return Results.Ok(new
                CreateGameSessionResponse(
                    gameSession.Id, 
                    gameSession.LeftSnippet,
                    gameSession.RightSnippet));
        } 
        
    }
}