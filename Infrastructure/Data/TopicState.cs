namespace TeleBrief.Infrastructure.Data;

public class TopicState
{
    public int Id { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int State { get; set; }
    public string LastAnalysis { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}