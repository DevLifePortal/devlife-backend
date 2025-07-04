using System.Net.Http.Headers;
using System.Security.Claims;
using DevLife.Infrastructure.Database.Redis;
using DevLife.Infrastructure.Database.Redis.Github;

namespace DevLife.Features.RepositoryAnalyze;

public class GetGithubRepos
{
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/github/repos", Handler)
                .RequireAuthorization()
                .WithTags("Developer Insights");
        }

        private static async Task<IResult> Handler(
            ClaimsPrincipal user,
            IGitHubTokenStorage tokenStore,
            HttpClient http,
            CancellationToken cancellationToken)
        {
            var userName = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userName))
                return Results.Unauthorized();

            var token = await tokenStore.GetTokenAsync(userName, cancellationToken);
            if (string.IsNullOrEmpty(token))
                return Results.Unauthorized();

            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            http.DefaultRequestHeaders.UserAgent.ParseAdd("DevLife");

            var response = await http.GetAsync("https://api.github.com/user/repos", cancellationToken);
            if (!response.IsSuccessStatusCode)
                return Results.BadRequest("Failed to fetch repositories");

            var repos = await response.Content.ReadFromJsonAsync<List<GitHubRepo>>(cancellationToken: cancellationToken);
            return Results.Ok(repos);
        }
    }
    public record GitHubRepo(string Name, string Full_Name, string Html_Url, string Description);

}