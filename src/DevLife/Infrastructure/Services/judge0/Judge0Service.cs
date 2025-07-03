using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace DevLife.Infrastructure.Services.judge0;

public class Judge0Service
{
    private readonly HttpClient _http;
    private readonly Judge0Options _options;

    public Judge0Service(HttpClient http, IOptions<Judge0Options> options)
    {
        _http = http;
        _options = options.Value;
        _http.BaseAddress = new Uri("https://judge0-ce.p.rapidapi.com/");
        _http.DefaultRequestHeaders.Add("X-RapidAPI-Key", _options.Key); 
        _http.DefaultRequestHeaders.Add("X-RapidAPI-Host", "judge0-ce.p.rapidapi.com");
    }

    public async Task<Judge0Response> ExecuteAsync(string sourceCode, string language, CancellationToken cancellationToken)
    {
        var languageId = language switch
        {
            "python" => 71,
            "javascript" => 63,
            "csharp" => 51,
            "java" => 62,
            _ => throw new ArgumentOutOfRangeException(nameof(language), $"Unsupported language: {language}")
        };

        var request = new
        {
            source_code = sourceCode,
            language_id = languageId,
            stdin = "",
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync("submissions?base64_encoded=false&wait=true", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Judge0 error: {response.StatusCode}");

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<Judge0Response>(responseBody)!;
    }
}