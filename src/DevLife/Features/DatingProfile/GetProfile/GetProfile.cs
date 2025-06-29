using DevLife.Infrastructure.Database.Mongo.Repository;
using DevLife.Infrastructure.Models.Entities;
using MongoDB.Driver.Linq;

namespace DevLife.Features.DatingProfile.GetProfile;

public class GetProfile
{
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/profile/{Id:guid}", Handler)
                .WithTags("Dev Dating");
        }

        private static async Task<IResult> Handler(
                Guid Id,
                IMongoRepository<UserProfile> repository)
        {
            var profile = repository
                .AsQueryable()
                .FirstOrDefaultAsync(p => p.Id == Id);

            return profile is null
                ? Results.NotFound($"Profile with Id{Id} not found.")
                : Results.Ok(profile);
        }
    }
}