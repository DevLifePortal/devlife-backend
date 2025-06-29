
using DevLife.Features.Auth.GitHub;
using DevLife.Infrastructure.Services.GitHub;
using OpenAI_API.Moderation;

namespace DevLife.Features.RepositoryAnalyze;

using DeveloperAnalysisResult = GetRepositoryAnalyze.DeveloperAnalysisResult;

public class GetDeveloperCard
{
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/github/GetDeveloperCard", Handler)
                .RequireAuthorization()
                .WithTags("Developer Insights");
        }

        private static async Task<IResult> Handler(
            DeveloperAnalysisResult analysisResult,
            DeveloperCardGenerator developerCardGenerator)
        {
            var imageBytes = developerCardGenerator.GenerateCard(analysisResult);
            return Results.File(imageBytes, "image/png");
        }
    }
}