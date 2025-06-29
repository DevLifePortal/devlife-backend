namespace DevLife.Infrastructure.Services.CodeWars;

public class CodewarsTaskSelector
{
    private readonly IReadOnlyList<MetaTask> _tasks;
    private readonly Random _random = new();

    public CodewarsTaskSelector()
    {
        _tasks = new List<MetaTask>
        {
            new("find-the-odd-int", "Find The Odd Int", new[] { "csharp", "javascript" }, "medium", new[] { "bitwise", "algorithms" }),
            new("convert-a-string-to-a-number", "Convert String to Number", new[] { "python", "javascript" }, "easy", new[] { "parsing" }),
            new("is-this-a-triangle", "Is This a Triangle?", new[] { "csharp", "python" }, "easy", new[] { "geometry", "logic" }),
            new("sum-of-digits-digital-root", "Digital Root", new[] { "javascript", "python" }, "medium", new[] { "recursion" }),
        };
    }

    public string? GetRandomSlug(string language, string difficulty)
    {
        var filtered = _tasks
            .Where(t => t.Languages.Contains(language, StringComparer.OrdinalIgnoreCase)
                        && string.Equals(t.Difficulty, difficulty, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!filtered.Any()) return null;

        return filtered[_random.Next(filtered.Count)].Slug;
    }

    private record MetaTask(
        string Slug,
        string Title,
        string[] Languages,
        string Difficulty,
        string[] Tags
    );
}
