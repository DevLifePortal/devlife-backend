using System.Text.Json.Serialization;

namespace DevLife.Infrastructure.Services.judge0;

public class Judge0Response
{
    [JsonPropertyName("stdout")]
    public string? Stdout { get; set; }

    [JsonPropertyName("stderr")]
    public string? Stderr { get; set; }

    [JsonPropertyName("compile_output")]
    public string? CompileOutput { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("status")]
    public Judge0Status Status { get; set; }
    
    public class Judge0Status
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
