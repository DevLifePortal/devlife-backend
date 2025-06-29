using System.Security.Claims;
using DevLife.Infrastructure.Database.Mongo.Repository;
using DevLife.Infrastructure.Database.Postgres.Repository;
using DevLife.Infrastructure.Models.Entities;
using DevLife.Infrastructure.Services.judge0;
using DevLife.Infrastructure.Services.OpenAI;
using MongoDB.Driver.Linq;

namespace DevLife.Features.CodeRoast;

public class SubmitSolution
{
    public record SubmitCommand(
        Guid TaskId,
        string Language,
        string Code
    );
    
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/code-roast/submit", Handler)
                .RequireAuthorization()
                .WithTags("Challenge");
        }
        private static async Task<IResult> Handler(
            SubmitCommand command,
            ClaimsPrincipal userPrincipal,
            IMongoRepository<CodingTask> codingTaskRepository,
            Judge0Service judge0,
            ChatGptService chatGptService,
            CancellationToken cancellationToken)
        {
            var claimUserName = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrWhiteSpace(claimUserName))
                return Results.Unauthorized();
            
            var task = await codingTaskRepository
                .Where(t => t.Id == command.TaskId)
                .FirstOrDefaultAsync(cancellationToken);
            
            var result = await judge0.ExecuteAsync(command.Code, command.Language, cancellationToken);

            var output = result.Stdout?.Trim();
            var error = result.Stderr ?? result.CompileOutput ?? result.Message;
            var status = result.Status?.Description ?? "Unknown";

            var roast = await chatGptService.GetCodeFeedBack(
                task.Description,
                command.Code,
                output ?? error ?? "No output"
            );

            return Results.Ok(new Response(output, error, status, roast));
        }
    }
    private record Response(
        string? Output,
        string? Error,
        string Status,
        string AiComment 
    );
    
}