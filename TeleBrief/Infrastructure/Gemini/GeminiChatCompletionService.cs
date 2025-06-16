using System.Net.Http.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace TeleBrief.Infrastructure.Gemini;

public class GeminiChatCompletionService(
    string endpoint,
    string model,
    string key,
    IReadOnlyDictionary<string, object?> attributes)
    : IChatCompletionService
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri(endpoint)
    };

    public IReadOnlyDictionary<string, object?> Attributes { get; } = attributes;

    public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory,
        PromptExecutionSettings? executionSettings = null,
        Kernel? kernel = null, CancellationToken cancellationToken = new())
    {
        var messages = chatHistory.Select(m => new
        {
            role = m.Role.ToString().ToLower(),
            parts = new[] { new { text = m.Content } }
        });

        var payload = new
        {
            contents = messages
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"models/{model}:generateContent?key={key}",
            payload,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken);
        var text = result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? "";

        return new List<ChatMessageContent>
        {
            new(AuthorRole.Assistant, text)
        };
    }

    public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory,
        PromptExecutionSettings? executionSettings = null, Kernel? kernel = null,
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }
}