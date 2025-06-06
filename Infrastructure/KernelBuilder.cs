using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using TeleBrief.Infrastructure.Gemini;
using TeleBrief.Infrastructure.Plugins;

namespace TeleBrief.Infrastructure;

public static class KernelBuilder
{
    public static Kernel BuildKernel(AppConfig config)
    {
        var builder = Kernel.CreateBuilder();

        if (!string.IsNullOrEmpty(config.Gemini.Key))
            builder.Services.AddSingleton<IChatCompletionService>(new GeminiChatCompletionService(
                config.Gemini.Endpoint, config.Gemini.Model, config.Gemini.Key, new Dictionary<string, object?>()));

        if (!string.IsNullOrEmpty(config.AzureOpenAiDeployment.Key))
            builder
                .AddAzureOpenAIChatCompletion(config.AzureOpenAiDeployment.Name, config.AzureOpenAiDeployment.Endpoint,
                    config.AzureOpenAiDeployment.Key)
                .Build();

        var kernel = builder.Build();

        kernel.ImportPluginFromObject(new DatePlugin(), "Date");

        if (kernel is null)
            throw new ApplicationException(
                "You need to configure either Gemini or Azure OpenAI deployment in app.settings.json or local.settings.json. Here is how it might look like:\n" +
                "{\n" +
                "  \"Gemini\": {\n" +
                "    \"Key\": \"your-gemini-key\",\n" +
                "    \"Endpoint\": \"https://your-gemini-endpoint\",\n" +
                "    \"Model\": \"gemini-1.5-flash\"\n" +
                "  },\n" +
                "  \"AzureOpenAiDeployment\": {\n" +
                "    \"Key\": \"your-azure-openai-key\",\n" +
                "    \"Endpoint\": \"https://your-azure-openai-endpoint\",\n" +
                "    \"Name\": \"your-azure-openai-deployment-name\"\n" +
                "  }\n" +
                "}");

        return kernel;
    }
}