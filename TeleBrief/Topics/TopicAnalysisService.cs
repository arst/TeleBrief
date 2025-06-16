using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using TeleBrief.Infrastructure;
using TeleBrief.Infrastructure.Data;
using TeleBrief.Infrastructure.Memory;

namespace TeleBrief.Topics;

public class TopicAnalysisService
{
    private readonly AppConfig _config;
    private readonly AppDbContext _dbContext;
    private readonly FactStore _factStore;
    private readonly Kernel _kernel;

    public static int ParseStateLine(string? line, int defaultState)
    {
        if (string.IsNullOrWhiteSpace(line)) return defaultState;

        var stateText = line;
        var colonIndex = line.IndexOf(':');
        if (colonIndex >= 0)
            stateText = line[(colonIndex + 1)..].Trim();

        return int.TryParse(stateText, out var state) ? state : defaultState;
    }

    public TopicAnalysisService(AppDbContext dbContext, Kernel kernel, AppConfig config, FactStore factStore)
    {
        _dbContext = dbContext;
        _kernel = kernel;
        _config = config;
        _factStore = factStore;
    }

    public async Task<List<TopicState>> AnalyzeTopics(string summary)
    {
        var currentStates = new List<TopicState>();

        foreach (var topic in _config.Topics)
        {
            var facts = await _factStore.GetFacts($"{topic.Name} {topic.Description}");
            var factsText = string.Join("\n", facts.OrderBy(f => f.Date).Select(f => f.Text));

            var currentState = await _dbContext.TopicStates
                .FirstOrDefaultAsync(t => t.TopicName == topic.Name);

            var analysisPrompt = $$$"""
                                    Metadata:
                                    Today is: {{Date.GetTodayDate}}
                                    Data:
                                    Previous state: {{{currentState?.State ?? 50}}} (1 being "everything is fine", 100 being "it's really bad")
                                    Previous analysis: {{{currentState?.LastAnalysis ?? "No previous analysis"}}}
                                    Here are some recent news related to the topic:
                                    {{{factsText}}}

                                    Task:

                                    Based on the following summary, analyze the state of "{{{topic.Name}}}".
                                    {{{topic.Description}}}

                                    {{{topic.AnalysisPrompt}}}

                                    Summary:
                                    {{{summary}}}

                                    Respond in this exact format:
                                    State: [number between 1 and 100]
                                    Analysis: [2-3 sentences explaining the change in state]
                                    """;

            var result = await _kernel.InvokePromptAsync(analysisPrompt);
            var analysisText = result.GetValue<string>();

            if (string.IsNullOrWhiteSpace(analysisText))
                continue;

            var lines = analysisText.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            var newState = ParseStateLine(lines.FirstOrDefault() ?? string.Empty, currentState?.State ?? 50);

            var analysis = lines.Length > 1
                ? string.Join("\n", lines.Skip(1))
                : string.Empty;
            analysis = analysis.Replace("Analysis:", "").Trim();

            if (currentState == null)
            {
                currentState = new TopicState
                {
                    TopicName = topic.Name
                };
                _dbContext.TopicStates.Add(currentState);
            }

            currentState.State = newState;
            currentState.LastAnalysis = analysis;
            currentState.LastUpdated = DateTime.UtcNow;
            currentStates.Add(currentState);
        }

        await _dbContext.SaveChangesAsync();

        return currentStates;
    }
}