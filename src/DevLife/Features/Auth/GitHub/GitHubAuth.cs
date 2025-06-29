using System.Net.Http.Headers;
using System.Security.Claims;
using DevLife.Infrastructure.Database.Postgres.Repository;
using DevLife.Infrastructure.Database.Redis;
using DevLife.Infrastructure.Models.Entities;
using DevLife.Infrastructure.Services.GitHub;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;

namespace DevLife.Features.Auth.GitHub;

public class GitHubAuth
{
    public static class Endpoints
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/auth/github/login", Login)
                .WithTags("Auth");
            app.MapGet("/auth/github/callback", Callback)
                .WithTags("Auth");
        }

        private static IResult Login(IOptions<GitHubOAuthOptions> options)
        {
            var url = $"https://github.com/login/oauth/authorize" +
                      $"?client_id={options.Value.ClientId}" +
                      $"&redirect_uri={options.Value.Redirect}" +
                      $"&scope=repo%20read:user";
            return Results.Redirect(url);
        }

        private static async Task<IResult> Callback(
            string code,
            IOptions<GitHubOAuthOptions> options,
            ClaimsPrincipal userPrincipal,
            IPostgresRepository<User> userRepository,
            HttpClient http,
            IGitHubTokenStorage tokenStore,
            CancellationToken cancellationToken)
        {
            var userNamePrincipal = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userNamePrincipal is null)
                return Results.Unauthorized();

            var actuallyUser = await userRepository
                .Where(u => u.Username == userNamePrincipal)
                .FirstOrDefaultAsync(cancellationToken);
            
            var response = await http.PostAsync("https://github.com/login/oauth/access_token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", options.Value.ClientId },
                    { "client_secret", options.Value.ClientSecret },
                    { "code", code },
                    { "redirect_uri", options.Value.Redirect }
                }), cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var query = System.Web.HttpUtility.ParseQueryString(content);
            var accessToken = query["access_token"];

            if (string.IsNullOrEmpty(accessToken))
                return Results.BadRequest("No token");

            http.DefaultRequestHeaders.UserAgent.ParseAdd("DevLife");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var user = await http.GetFromJsonAsync<GitHubUser>("https://api.github.com/user",  cancellationToken);
            if (user == null)
                return Results.BadRequest("Failed to fetch user");

            await tokenStore.SetTokenAsync(actuallyUser.Username, accessToken, cancellationToken);

            return Results.Ok(new
            {
                Message = "Auth Success",
                GitHubId = user.Id,
                user.Login
            });
        }
    }
}
