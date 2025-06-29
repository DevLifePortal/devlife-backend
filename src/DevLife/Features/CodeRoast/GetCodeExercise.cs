using System.Security.Claims;
using DevLife.Infrastructure.Database.Mongo.Repository;
using DevLife.Infrastructure.Database.Postgres.Repository;
using DevLife.Infrastructure.Models.Abstractions;
using DevLife.Infrastructure.Models.Entities;
using DevLife.Infrastructure.Models.Enums;
using DevLife.Infrastructure.Services.CodeWars;
using Microsoft.EntityFrameworkCore;
namespace DevLife.Features.CodeRoast;

public class GetCodeExercise
{
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("code-roast/exercise", Handler)
                .RequireAuthorization()
                .WithTags("Challenge");
        }

        private static async Task<IResult> Handler(
            ClaimsPrincipal userPrincipal,
            IPostgresRepository<User> userRepository,
            IMongoRepository<CodingTask> codingTaskRepository,
            CodewarsClient client,
            CancellationToken cancellationToken)
        {
            var userName = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userName))
                return Results.Unauthorized();

            var user = await userRepository
                .Where(u => u.Username == userName)
                .FirstOrDefaultAsync(cancellationToken);

            string difficulty = "easy";
            if (user.ExperienceLevel is ExperienceLevel.Junior or ExperienceLevel.Middle)
                difficulty = "easy";
            else
                difficulty = "medium";

            var codingTask = await client.GetTaskBySlugAsync(new CodewarsTaskSelector()
                .GetRandomSlug(user.TechStack.ToString().ToLower(), difficulty), cancellationToken);

            await codingTaskRepository.AddAsync(codingTask, cancellationToken);
            
            return Results.Ok(codingTask);
        }
    }
}
