using Spectre.Console;
using System.Text;

namespace TeleBrief;

public static class SummaryRenderer
{
    public static void RenderStructuredSummary(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return;
        }
        
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var sections = new Dictionary<string, List<string>>();

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
                if (!string.IsNullOrWhiteSpace(fact))
                {
                    sections[currentCategory].Add(fact);
                }
            }
        }

        // Render
        foreach (var (category, facts) in sections)
        {
            var sb = new StringBuilder();
            foreach (var fact in facts)
            {
                sb.AppendLine($"[green]•[/] {Markup.Escape(fact)}");
            }

            AnsiConsole.Write(
                new Panel(sb.ToString().TrimEnd())
                    .BorderColor(Color.BlueViolet)
                    .Header($"[bold yellow]{Markup.Escape(category)}[/]", Justify.Center)
                    .Border(BoxBorder.Heavy)
                    .Padding(1, 1)
            );
        }
    }
}
