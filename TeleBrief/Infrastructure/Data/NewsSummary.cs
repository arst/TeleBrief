namespace TeleBrief.Infrastructure.Data;

public class NewsSummary
{
    public int Id { get; set; }
    public string Summary { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}