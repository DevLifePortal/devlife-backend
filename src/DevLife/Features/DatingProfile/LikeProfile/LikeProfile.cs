using System.Security.Claims;
using DevLife.Infrastructure.Database.Mongo.Repository;
using DevLife.Infrastructure.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevLife.Features.DatingProfile.LikeProfile;

public class LikeProfile
{
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/profile/like/{targetId:Guid}", Handler)
                .RequireAuthorization()
                .WithTags("Dev Dating");
        }

        private static async Task<IResult> Handler(
            Guid targetId,
            ClaimsPrincipal userPrincipal,
            IMongoRepository<UserProfile> userProfileRepository,
            IMongoRepository<ProfileInteraction> profileInteractionRepository,
            CancellationToken cancellationToken)
        {
            var userName =  userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userName is null)
                return Results.Unauthorized();
            
            var user = await userProfileRepository
                .Where(u => u.Username == userName)
                .FirstOrDefaultAsync(cancellationToken);

            var interaction = ProfileInteraction.Create(user.Id, targetId);
            
            await profileInteractionRepository.AddAsync(interaction, cancellationToken);

            return Results.Ok("liked");
        }
    }
}