using Spectre.Console.Cli;
using TeleBrief.Infrastructure.Memory;
using TeleBrief.News;

namespace TeleBrief.Commands;

public class NewsCommand(NewsService newsService, FactStore factStore)
    : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var summary = await newsService.GetTodaySummary();
        var categorizedFacts = SummaryParser.Parse(summary);

        foreach (var (category, facts) in categorizedFacts)
        foreach (var fact in facts)
            await factStore.AddFact(new Fact
            {
                Id = Guid.NewGuid().ToString(),
                Category = category,
                Text = fact,
                Date = DateOnly.FromDateTime(DateTime.UtcNow)
            });

        Renderer.RenderSummary(categorizedFacts);

        return 0;
    }
}
