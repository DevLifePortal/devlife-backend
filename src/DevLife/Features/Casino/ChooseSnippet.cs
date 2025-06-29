using System.Security.Claims;
using DevLife.Infrastructure.Database.Mongo.Repository;
using DevLife.Infrastructure.Database.Postgres.Repository;
using DevLife.Infrastructure.Models.Entities;
using DevLife.Infrastructure.Models.Enums;
using DevLife.Infrastructure.Services.Jwt;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;

namespace DevLife.Features.Casino;

public class ChooseSnippet
{
    private record ChooseSnippetCommand(Guid SessionId, string Choice);
    
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("casino/choose", Handler)
                .RequireAuthorization()
                .WithTags("Code Casino");
        }

        private static async Task<IResult> Handler(
            ChooseSnippetCommand command,
            ClaimsPrincipal userPrincipal,
            IMongoRepository<GameSession> sessionRepository,
            IPostgresRepository<CasinoGameResult> gameResultRepository,
            IPostgresRepository<User> userRepository,
            CancellationToken cancellationToken)
        {
            var userNameClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userNameClaim))
                return Results.Unauthorized();
            
            var session = await MongoQueryable.FirstOrDefaultAsync(sessionRepository
                .AsQueryable(), u => u.Id == command.SessionId, cancellationToken);

            if (session is null)
                return Results.NotFound("Session not found");
            if (command.Choice is not ("Left" or "Right"))
                return Results.BadRequest("Write Left or Right");

            var user = await
                EntityFrameworkQueryableExtensions
                    .FirstOrDefaultAsync(
                        userRepository.Where(x => x.Id == session.UserId),
                        cancellationToken);

            var isCorrect = session.CorrectCodeSlot.ToString() == command.Choice;

            if (isCorrect)
                user.Balance += session.Bet * 2;
            else
                user.Balance -= session.Bet ;

            await gameResultRepository
                .AddAsync(CasinoGameResult.Create(
                        user.Id,
                        userAnswer: command.Choice == "Left" ? CodeSlot.Left : CodeSlot.Right,
                        session.CorrectCodeSlot,
                        isCorrect,
                        session.Bet,
                        user.Balance), cancellationToken);
            
            await userRepository.UpdateAsync(user, cancellationToken);
            await sessionRepository.DeleteAsync([session], cancellationToken);
            return isCorrect ? Results.Ok() : Results.NotFound();
        }
    }
}