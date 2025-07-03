using DevLife.Infrastructure.Database.Mongo.Repository;
using DevLife.Infrastructure.Models.Entities;
using MongoDB.Driver.Linq;

namespace DevLife.Features.DatingProfile.GetMessages;

public class GetMessages
{
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/chat/{matchId:guid}", Handler)
                .RequireAuthorization()
                .WithTags("Dev Dating");
        }
        private static async Task<IResult> Handler(
            Guid matchId,
            IMongoRepository<Message> messageRepository,
            CancellationToken cancellationToken)
        {
            var messages = await messageRepository
                .Where(m => m.MatchId == matchId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync(cancellationToken);
            return Results.Ok(messages);
        }
    }
}