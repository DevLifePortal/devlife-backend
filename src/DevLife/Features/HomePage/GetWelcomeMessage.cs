using System.Security.Claims;
using DevLife.Infrastructure.Database.Postgres.Repository;
using DevLife.Infrastructure.Models.Entities;
using DevLife.Infrastructure.Services.OpenAI;
using Microsoft.EntityFrameworkCore;

namespace DevLife.Features.HomePage;

public class GetWelcomeMessage
{
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/home", Handler)
                .WithTags("HomePage");
        }

        private static async Task<IResult> Handler(
                ClaimsPrincipal userPrincipal,
                IPostgresRepository<User> userRepository,
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

            var recommendation = await chatGptService
                .GetZodiacSignRecommendation(user.ZodiacSign.Name);
            
            return Results.Ok(new
            {
                message = $"Hello, {user.FullName}! ♏ {user.ZodiacSign.Name}, today {recommendation}",
            });
        }
    }
}