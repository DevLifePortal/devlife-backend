using System.Security.Claims;
using DevLife.Infrastructure.Database.Mongo.Repository;
using DevLife.Infrastructure.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevLife.Features.DatingProfile.Setup;

public class ProfileRegister
{
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/profile/setup", Handler)
                .RequireAuthorization()
                .WithTags("Dev Dating");
        }

        private static async Task<IResult> Handler(
            RegisterProfileCommand command,
            ClaimsPrincipal userPrincipal,
            IMongoRepository<UserProfile> repository,
            CancellationToken cancellationToken)
        {
            
            var userName = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userName is null)
                return Results.Unauthorized();
            
            var profile = UserProfile.Create(
                command.Username,
                command.Gender,
                command.Preference,
                command.Bio,
                command.TechStack,
                command.personalityType);

            await repository.AddAsync(profile, cancellationToken);

            return Results.Ok(profile.Id);
        }
    }   
}