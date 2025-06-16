using Spectre.Console.Cli;
using TeleBrief.News;
using TeleBrief.Topics;

namespace TeleBrief.Commands;

public class HeartbeatCommand(NewsService newsService, TopicAnalysisService topicAnalysisService) : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var summary = await newsService.GetTodaySummary();
        var currentStates = await topicAnalysisService.AnalyzeTopics(summary);
        Renderer.RenderTopics(currentStates);

        return 0;
    }
}