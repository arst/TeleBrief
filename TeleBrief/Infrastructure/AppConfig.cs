using TeleBrief.Topics;

namespace TeleBrief.Infrastructure;

public class AppConfig
{
    public List<string> TelegramChannels { get; set; } = new();

    public int BatchSize { get; set; }

    public List<TopicConfig> Topics { get; set; } = new();

    public AzureOpenAiDeploymentOptions AzureOpenAiDeployment { get; set; } = new();

    public GeminiOptions Gemini { get; set; } = new();

    public class GeminiOptions
    {
        public string Key { get; set; } = string.Empty;

        public string Endpoint { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;
    }

    public class AzureOpenAiDeploymentOptions
    {
        public string Name { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
    }
}