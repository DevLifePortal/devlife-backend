using System.Text.RegularExpressions;
using DevLife.Infrastructure.Database.Mongo.Repository;
using DevLife.Infrastructure.Models.Entities;
using MongoDB.Driver.Linq;
using Match = DevLife.Infrastructure.Models.Entities.Match;

namespace DevLife.Features.DatingProfile.AutoMatch;

public class AutoMatch
{
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("profile/match/{profileId:guid}", Handler)
                .RequireAuthorization()
                .WithTags("Dev Dating");
        }

        private static async Task<IResult> Handler(
            Guid profileId,
            IMongoRepository<ProfileInteraction> interactionRepo,
            IMongoRepository<Match> matchRepo,
            CancellationToken cancellationToken)
        {
            var outgoingLikes = interactionRepo.AsQueryable()
                .Where(i => i.SourceProfileId == profileId)
                .Select(i => i.TargetProfileId)
                .ToList();

            var incomingLikes = interactionRepo.AsQueryable()
                .Where(i => i.TargetProfileId == profileId)
                .Select(i => i.SourceProfileId)
                .ToList();

            var matchedUserId = outgoingLikes.Intersect(incomingLikes).FirstOrDefault();

            if (matchedUserId == Guid.Empty)
                return Results.NoContent();

            var existingMatch = matchRepo.AsQueryable()
                .FirstOrDefaultAsync(m =>
                    (m.Profile1Id == profileId && m.Profile2Id == matchedUserId) ||
                    (m.Profile1Id == matchedUserId && m.Profile2Id == profileId), cancellationToken);

            if (existingMatch is not null)
                return Results.Ok(existingMatch.Id); 

            var match = new Match
            {
                Profile1Id = profileId,
                Profile2Id = matchedUserId
            };

            await matchRepo.AddAsync(match, cancellationToken);

            return Results.Ok(match.Id);
        }
    }
}