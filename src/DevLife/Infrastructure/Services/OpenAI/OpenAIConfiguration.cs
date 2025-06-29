namespace DevLife.Infrastructure.Services.OpenAI;

public class OpenAiConfiguration
{
    public string ApiKey { get; set; }
    public string Model { get; set; }
    public string BaseUrl { get; set; } = "https://api.openai.com/v1/chat/completions";
}