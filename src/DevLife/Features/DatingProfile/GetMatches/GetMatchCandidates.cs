using System.Security.Claims;
using DevLife.Infrastructure.Database.Mongo.Repository;
using DevLife.Infrastructure.Models.Entities;
using DevLife.Infrastructure.Models.Enums;

namespace DevLife.Features.DatingProfile.GetMatches;

public class GetMatchCandidates
{
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("profile/matches", Handler)
                .RequireAuthorization()
                .WithTags("Dev Dating");
        }

        private static async Task<IResult> Handler(
            ClaimsPrincipal userPrincipal,
            IMongoRepository<UserProfile> repository)
        {
            var userName = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userName is null)
                return Results.Unauthorized();
        
            var currentUser = repository.AsQueryable().FirstOrDefault(p => p.Username == userName);
            if (currentUser is null)
                return Results.NotFound("Профиль не найден");
            
            var candidates = repository.AsQueryable()
                .Where(p =>
                        p.Id != currentUser.Id &&
                        (currentUser.Preference == GenderPreference.All || p.Gender.ToString() == currentUser.Preference.ToString()))
                .OrderByDescending(p => p.TechStack.Intersect(currentUser.TechStack).Count())
                .Take(10)
                .ToList();

            return Results.Ok(candidates);
        }
    }
}