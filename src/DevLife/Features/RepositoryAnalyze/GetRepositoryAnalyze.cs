using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using DevLife.Infrastructure.Database.Redis;
using DevLife.Infrastructure.Services.OpenAI;

namespace DevLife.Features.Auth.GitHub;

public class GetRepositoryAnalyze
{
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/github/analyze-repo", Handler)
                .RequireAuthorization()
                .WithTags("Developer Insights");
        }

        public record AnalyzeRepoRequest(string RepoUrl);

        private static readonly string[] AllowedExtensions = new[] { ".cs", ".js", ".ts", ".py", ".java", ".cpp", ".c", ".go", ".rb" };
        
        // aq AI gamoviyene ver movityuebi
        private static async Task<IResult> Handler(
            ClaimsPrincipal user,
            AnalyzeRepoRequest request,
            IGitHubTokenStorage tokenStore,
            HttpClient http,
            ChatGptService chatGptService,
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

            if (!TryParseGitHubRepoUrl(request.RepoUrl, out var owner, out var repo))
                return Results.BadRequest("Invalid GitHub repository URL");

            var allFiles = new List<GitHubContentItem>();
            await FetchRepoContentsRecursive(http, owner, repo, "", allFiles, cancellationToken);

            var codeFiles = allFiles
                .Where(f => AllowedExtensions.Any(ext => f.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (codeFiles.Count == 0)
                return Results.BadRequest("No code files found in repository");

            var filesToAnalyze = codeFiles.Take(10);

            var sb = new StringBuilder();
            foreach (var file in filesToAnalyze)
            {
                var content = await GetFileContent(http, file.Download_Url, cancellationToken);
                sb.AppendLine($"// File: {file.Path}");
                sb.AppendLine(content);
                sb.AppendLine("\n\n");
            }
            var combinedCode = sb.ToString();

            // 2. Получаем последние 20 коммитов
            var commitsResponse = await http.GetAsync($"https://api.github.com/repos/{owner}/{repo}/commits?per_page=20", cancellationToken);
            if (!commitsResponse.IsSuccessStatusCode)
                return Results.BadRequest("Failed to fetch commits");

            var commitsJson = await commitsResponse.Content.ReadFromJsonAsync<List<GitHubCommit>>(cancellationToken: cancellationToken);
            var commitMessages = string.Join("\n", commitsJson?.Select(c => c.Commit.Message) ?? Enumerable.Empty<string>());
            
            
            var gptJsonResponse = await chatGptService.AnalyzeDeveloperTypeJson(combinedCode, commitMessages);

            // 5. Парсим JSON из GPT
            DeveloperAnalysisResult? analysisResult = null;
            try
            {
                analysisResult = System.Text.Json.JsonSerializer.Deserialize<DeveloperAnalysisResult>(gptJsonResponse,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                return Results.BadRequest("Failed to parse GPT response");
            }

            if (analysisResult == null)
                return Results.BadRequest("GPT returned empty analysis");

            return Results.Ok(analysisResult);
        }

        private static async Task FetchRepoContentsRecursive(HttpClient http, string owner, string repo, string path, List<GitHubContentItem> collector, CancellationToken ct)
        {
            var url = $"https://api.github.com/repos/{owner}/{repo}/contents/{path}";
            var response = await http.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync(ct);

            if (json.TrimStart().StartsWith("["))
            {
                var items = System.Text.Json.JsonSerializer.Deserialize<List<GitHubContentItem>>(json, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (items == null) return;

                foreach (var item in items)
                {
                    if (item.Type == "file")
                    {
                        collector.Add(item);
                    }
                    else if (item.Type == "dir")
                    {
                        await FetchRepoContentsRecursive(http, owner, repo, item.Path, collector, ct);
                    }
                }
            }
            else
            {
                var fileItem = System.Text.Json.JsonSerializer.Deserialize<GitHubContentItem>(json, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (fileItem != null && fileItem.Type == "file")
                    collector.Add(fileItem);
            }
        }

        private static async Task<string> GetFileContent(HttpClient http, string downloadUrl, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(downloadUrl)) return string.Empty;

            var content = await http.GetStringAsync(downloadUrl, ct);
            return content;
        }

        private static bool TryParseGitHubRepoUrl(string url, out string owner, out string repo)
        {
            owner = null!;
            repo = null!;
            try
            {
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Trim('/').Split('/');
                if (segments.Length < 2) return false;
                owner = segments[0];
                repo = segments[1];
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GenerateDeveloperTypePrompt(string code, string commitMessages)
        {
            return
                "You are an expert software engineer and psychologist who analyzes developer personality and coding style.\n\n" +
                "Analyze the following information extracted from a GitHub repository:\n" +
                "- Code snippets:\n" + code + "\n\n" +
                "- Commit messages:\n" + commitMessages + "\n\n" +
                "Evaluate the developer's style and personality based on these parameters:\n" +
                "1. Commit message style\n" +
                "2. Code commenting\n" +
                "3. Variable naming conventions\n" +
                "4. Project structure\n\n" +
                "Provide the following results:\n" +
                "- Personality type with a local flavor phrase (e.g., 'შენ ხარ Chaotic Debugger')\n" +
                "- Strengths and weaknesses of this developer\n" +
                "- Name up to 3 celebrity developers with a similar coding style\n" +
                "- A short, catchy description suitable for a shareable card\n\n" +
                "Format your response in JSON with fields: personalityType, strengths, weaknesses, celebrityDevelopers, cardDescription.\n\n" +
                "Here is the data to analyze:\n\n" +
                "Code:\n" + code + "\n\n" +
                "Commit messages:\n" + commitMessages;
        }
    }

    private class GitHubContentItem
    {
        public string Name { get; set; } = "";
        public string Path { get; set; } = "";
        public string Type { get; set; } = ""; // file or dir
        public string Download_Url { get; set; } = "";
    }

    private class GitHubCommit
    {
        public CommitInfo Commit { get; set; } = null!;
    }

    private class CommitInfo
    {
        public string Message { get; set; } = "";
    }

    public class DeveloperAnalysisResult
    {
        public string PersonalityType { get; set; } = "";
        public string Strengths { get; set; } = "";
        public string Weaknesses { get; set; } = "";
        public List<string> CelebrityDevelopers { get; set; } = new();
        public string CardDescription { get; set; } = "";
    }
}
