using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AIChatbot.Models;

namespace AIChatbot.Services;

public class AIService
{
    private readonly IHttpClientFactory _factory;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly string _endpoint;

    public AIService(IHttpClientFactory factory, IConfiguration config)
    {
        _factory = factory;
        _apiKey = config["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "";
        _model = config["OpenAI:Model"] ?? Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-3.5-turbo";
        _endpoint = config["OpenAI:Endpoint"] ?? Environment.GetEnvironmentVariable("OPENAI_ENDPOINT") ?? "https://api.openai.com/v1/chat/completions";
    }

    public async Task<string> GetChatCompletionAsync(IReadOnlyList<ChatMessage> messages)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new InvalidOperationException("OpenAI API key missing.");

        var http = _factory.CreateClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var payload = new
        {
            model = _model,
            messages = messages.Select(m => new { role = m.Role, content = m.Content }).ToList(),
            temperature = 0.7
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await http.PostAsync(_endpoint, content);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        var doc = await JsonDocument.ParseAsync(stream);
        var reply = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return reply ?? "(no content)";
    }
}