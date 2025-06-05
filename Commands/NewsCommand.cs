using Spectre.Console.Cli;
using TeleBrief.News;

namespace TeleBrief.Commands;

public class NewsCommand(NewsService newsService)
    : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var summary = await newsService.GetTodaySummary();
        Renderer.RenderSummary(summary);

        return 0;
    }
}