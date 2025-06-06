using System.Text;
using Spectre.Console;
using TeleBrief.Infrastructure.Data;

namespace TeleBrief;

public static class Renderer
{
    public static void RenderSummary(Dictionary<string, List<string>> sections)
    {
        if (sections.Count == 0) return;

        foreach (var (category, facts) in sections)
        {
            var sb = new StringBuilder();
            foreach (var fact in facts) sb.AppendLine($"[green]•[/] {Markup.Escape(fact)}");

            AnsiConsole.Write(
                new Panel(sb.ToString().TrimEnd())
                    .BorderColor(Color.BlueViolet)
                    .Header($"[bold yellow]{Markup.Escape(category)}[/]", Justify.Center)
                    .Border(BoxBorder.Heavy)
                    .Padding(1, 1)
            );
        }
    }

    public static void RenderTopics(List<TopicState> topicStates)
    {
        if (topicStates.Count == 0) return;

        AnsiConsole.MarkupLine("[blue]Topic Analysis:[/]");
        AnsiConsole.WriteLine();

        var table = new Table();
        table.AddColumn("Topic");
        table.AddColumn(new TableColumn("State").Centered());
        table.AddColumn("Analysis");
        table.Expand();

        foreach (var state in topicStates)
        {
            var red = Math.Min(255, Math.Max(0, (int)(255 * (state.State / 100.0))));
            var green = Math.Min(255, Math.Max(0, (int)(255 * (1 - state.State / 100.0))));

            var stateBox = new Panel($"[white]{state.State}[/]")
                .BorderColor(new Color((byte)red, (byte)green, 0))
                .Padding(0, 1);

            table.AddRow(
                new Text(state.TopicName),
                new Padder(stateBox),
                new Text(state.LastAnalysis)
            );
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }
}