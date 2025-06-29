using DevLife.Infrastructure.Database.Mongo.Repository;
using DevLife.Infrastructure.Models.Entities;

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
            IMongoRepository<Message> messageRepo)
        {
            var messages = messageRepo.AsQueryable()
                .Where(m => m.MatchId == matchId)
                .OrderBy(m => m.Timestamp)
                .ToList();
            return Results.Ok(messages);
        }
    }
}