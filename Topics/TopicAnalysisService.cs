using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using TeleBrief.Infrastructure;
using TeleBrief.Infrastructure.Data;

namespace TeleBrief.Topics;

public class TopicAnalysisService
{
    private readonly AppConfig _config;
    private readonly AppDbContext _dbContext;
    private readonly Kernel _kernel;

    public TopicAnalysisService(AppDbContext dbContext, Kernel kernel, AppConfig config)
    {
        _dbContext = dbContext;
        _kernel = kernel;
        _config = config;
    }

    public async Task<List<TopicState>> AnalyzeTopics(string summary)
    {
        var currentStates = new List<TopicState>();

        foreach (var topic in _config.Topics)
        {
            var currentState = await _dbContext.TopicStates
                .FirstOrDefaultAsync(t => t.TopicName == topic.Name);

            var analysisPrompt = $"""
                                  Previous state: {currentState?.State ?? 50} (1 being "everything is fine", 100 being "it's really bad")
                                  Previous analysis: {currentState?.LastAnalysis ?? "No previous analysis"}

                                  Based on the following summary, analyze the state of "{topic.Name}".
                                  {topic.Description}

                                  {topic.AnalysisPrompt}

                                  Summary:
                                  {summary}

                                  Respond in this exact format:
                                  State: [number between 1 and 100]
                                  Analysis: [2-3 sentences explaining the change in state]
                                  """;

            var result = await _kernel.InvokePromptAsync(analysisPrompt);
            var analysisText = result.GetValue<string>();

            if (string.IsNullOrWhiteSpace(analysisText)) return currentStates;

            var lines = analysisText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var newState = int.Parse(lines[0].Split(':')[1].Trim());
            var analysis = string.Join("\n", lines.Skip(1)).Replace("Analysis:", "").Trim();

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