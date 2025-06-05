using Microsoft.SemanticKernel;
using Spectre.Console.Cli;

namespace TeleBrief.Commands;

public class NewsCommand : AsyncCommand
{
    private readonly AppConfig _appConfig;
    private readonly Kernel _kernel;

    public NewsCommand(AppConfig appConfig, Kernel kernel)
    {
        _appConfig = appConfig;
        _kernel = kernel;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var summarizeMessagesFunction = _kernel.CreateFunctionFromPrompt("Summarize the following Telegram messages. Ignore speculation or noise. Ignore reactions, comments and so on. Ignore commercials, promotions and ads. Always answer in English. Keep only meaningful facts:\n{{$input}}");
        var finalSummaryFunction = _kernel.CreateFunctionFromPrompt(
            """
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

        foreach (var channelName in _appConfig.TelegramChannels)
        {
            var messages = await ChannelScrapper.GetTodaysMessagesFromChannel(channelName);

            foreach (var messagesToSummarize in messages.Chunk(_appConfig.BatchSize))
            {
                var summary = await _kernel.InvokeAsync(summarizeMessagesFunction, new KernelArguments { ["input"] = string.Join("\n", messagesToSummarize) });
                var result = summary.GetValue<string>();
                if (!string.IsNullOrWhiteSpace(result))
                {
                    summaries.Add(result);
                }
            }
        }

        var finalSummary = await _kernel.InvokeAsync(finalSummaryFunction, new KernelArguments { ["input"] = string.Join("\n", summaries) });
        SummaryRenderer.RenderStructuredSummary(finalSummary.GetValue<string>());

        return 0;
    }
} 