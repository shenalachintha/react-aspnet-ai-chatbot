using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AIChatbot.Models;
using Microsoft.Extensions.Logging;

namespace AIChatbot.Services;

public class AIService
{
    private readonly IHttpClientFactory _factory;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly string _baseUrl;
    private readonly ILogger<AIService> _logger;

    public AIService(IHttpClientFactory factory, IConfiguration config, ILogger<AIService> logger)
    {
        _factory = factory;
        _apiKey = config["Gemini:ApiKey"] ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "";
        _model = config["Gemini:Model"] ?? Environment.GetEnvironmentVariable("GEMINI_MODEL") ?? "gemini-2.5-flash";
        _baseUrl = config["Gemini:BaseUrl"] ?? Environment.GetEnvironmentVariable("GEMINI_BASE_URL")
                   ?? "https://generativelanguage.googleapis.com";
        _logger = logger;
    }

    public async Task<string> GetChatCompletionAsync(IReadOnlyList<ChatMessage> messages)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new InvalidOperationException("Gemini API key missing. Set GEMINI_API_KEY environment variable or use __dotnet user-secrets__.");

        var http = _factory.CreateClient();
        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        http.Timeout = TimeSpan.FromSeconds(30);

        // Use v1 endpoint for newer Gemini models (fallback to v1beta if explicitly configured)
        string versionSegment = _baseUrl.Contains("/v1beta/", StringComparison.OrdinalIgnoreCase) ? "v1beta" : "v1";
        var baseTrim = _baseUrl.TrimEnd('/');
        var requestUrl = $"{baseTrim}/{versionSegment}/models/{_model}:generateContent?key={_apiKey}";

        var payload = new
        {
            contents = messages.Select(m => new
            {
                role = MapRole(m.Role),
                parts = new[] { new { text = m.Content } }
            }),
            generationConfig = new
            {
                temperature = 0.7,
                maxOutputTokens = 1024
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        try
        {
            _logger.LogDebug("Posting to Gemini endpoint {Url}", requestUrl);
            response = await http.PostAsync(requestUrl, content);
        }
        catch (TaskCanceledException ex) when (!ex.CancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Gemini request timed out.");
            throw new HttpRequestException("Upstream request timed out.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HTTP request to Gemini failed.");
            throw new HttpRequestException("Failed to call Gemini API.", ex);
        }

        var responseText = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Gemini returned {StatusCode}. Body: {Body}", (int)response.StatusCode, responseText);
            throw new HttpRequestException($"Gemini returned {(int)response.StatusCode} {response.ReasonPhrase}: {responseText}");
        }

        try
        {
            using var doc = JsonDocument.Parse(responseText);
            var root = doc.RootElement;

            // standard Gemini response shape: candidates[0].content.parts[0].text
            if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
            {
                var candidate = candidates[0];
                if (candidate.TryGetProperty("content", out var contentObj) &&
                    contentObj.TryGetProperty("parts", out var parts) &&
                    parts.GetArrayLength() > 0)
                {
                    var text = parts[0].GetProperty("text").GetString();
                    return text ?? "(no content)";
                }
            }

            _logger.LogError("Unexpected Gemini response shape. Body: {Body}", responseText);
            throw new InvalidOperationException("Failed to parse Gemini response.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse Gemini response as JSON. Body: {Body}", responseText);
            throw new InvalidOperationException("Failed to parse Gemini response.", ex);
        }
    }

    private static string MapRole(string role) => role?.ToLowerInvariant() switch
    {
        "assistant" => "model",
        "system" => "user",
        _ => "user"
    };
}       