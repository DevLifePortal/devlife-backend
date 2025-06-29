using DevLife.Infrastructure.Models.Entities;

namespace DevLife.Infrastructure.Services.CodeWars;

public class CodewarsClient
{
    private readonly HttpClient _http;

    public CodewarsClient(HttpClient http) => _http = http;

    public async Task<CodingTask?> GetTaskBySlugAsync(string? slug, CancellationToken ct)
    {
        var url = $"https://www.codewars.com/api/v1/code-challenges/{slug}";
        var response = await _http.GetFromJsonAsync<CodewarsResponse>(url, ct);
        if (response is null) return null;

        return new CodingTask
        {
            Id = Guid.NewGuid(),
            Title = response.Name,
            Description = response.Description,
            Language = response.Languages.FirstOrDefault() ?? "unknown",
            Difficulty = response.Rank?.Name ?? "unknown"
        };
    }

    private class CodewarsResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Languages { get; set; }
        public RankInfo Rank { get; set; }

        public class RankInfo
        {
            public string Name { get; set; } 
        }
    }
}