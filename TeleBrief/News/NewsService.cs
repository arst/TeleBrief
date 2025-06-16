using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using TeleBrief.Infrastructure;
using TeleBrief.Infrastructure.Data;
using TeleBrief.Infrastructure.Memory;

namespace TeleBrief.News;

public class NewsService(AppConfig appConfig, Kernel kernel, AppDbContext dbContext, FactStore factStore)
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    public async Task<string?> GetTodaySummary()
    {
        var cachedSummary = await dbContext.NewsSummaries
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        if (cachedSummary != null && DateTime.UtcNow - cachedSummary.CreatedAt <= CacheDuration)
            return cachedSummary.Summary;

        var summary = await GenerateNewSummary();

        if (!string.IsNullOrWhiteSpace(summary))
        {
            dbContext.NewsSummaries.Add(new NewsSummary
            {
                Summary = summary,
                CreatedAt = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();
        }

        return summary;
    }

    private async Task<string?> GenerateNewSummary()
    {
        var summarizeMessagesFunction = kernel.CreateFunctionFromPrompt(
            """
            Metadata:
            Today is: {{Date.GetTodayDate}}
            Task:
            Summarize the following Telegram messages. Ignore speculation or noise. Ignore reactions, comments and so on. Ignore commercials, promotions and ads. Always answer in English. Keep only meaningful facts.
            Today news:
            {{$input}}
            """);
        var finalSummaryFunction = kernel.CreateFunctionFromPrompt(
            """
            Metadata:
            Today is: {{Date.GetTodayDate}}
            Task:
            You are given several summaries of Telegram news.
            Your task is to:
            - Merge them into a single, concise summary
            - Eliminate duplicates and redundant facts
            - Group related facts under clearly labeled categories, use no more than 3-6 categories

            Follow this exact output format:
            Category Name:
            - Fact 1.
            - Fact 2.
            - Fact 3.

            Another Category:
            - Fact A.
            - Fact B.
            ...

            Use only this structure. No free text, no intro, no conclusion.

            Input:
            {{$input}}
            """);

        var summaries = new List<string>();

        foreach (var channelName in appConfig.TelegramChannels)
        {
            var messages = await ChannelScraper.GetTodaysMessagesFromChannel(channelName);

            foreach (var messagesToSummarize in messages.Chunk(appConfig.BatchSize))
            {
                var summary = await kernel.InvokeAsync(summarizeMessagesFunction,
                    new KernelArguments { ["input"] = string.Join("\n", messagesToSummarize) });
                var result = summary.GetValue<string>();
                if (!string.IsNullOrWhiteSpace(result)) summaries.Add(result);
            }
        }

        if (summaries.Count == 0) return null;

        var finalSummary = await kernel.InvokeAsync(finalSummaryFunction,
            new KernelArguments { ["input"] = string.Join("\n", summaries) });
        return finalSummary.GetValue<string>();
    }
}