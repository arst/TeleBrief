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
        var categorizedFacts = GetCategorizedFacts(summary);

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

    private static Dictionary<string, List<string>> GetCategorizedFacts(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return new Dictionary<string, List<string>>();

        var sections = new Dictionary<string, List<string>>();

        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        string? currentCategory = null;

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            // Detect new category
            if (line.EndsWith(":"))
            {
                currentCategory = line.TrimEnd(':').Trim();
                sections[currentCategory] = new List<string>();
            }
            // Add bullet to current category
            else if (line.StartsWith("-") && currentCategory != null)
            {
                var fact = line.TrimStart('-', ' ').Trim();
                if (!string.IsNullOrWhiteSpace(fact)) sections[currentCategory].Add(fact);
            }
        }

        return sections;
    }
}