using Microsoft.EntityFrameworkCore;

namespace TeleBrief.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
        var folder = Directory.GetCurrentDirectory();
        DbPath = Path.Join(folder, "telebrief.db");
    }

    public DbSet<TopicState> TopicStates { get; set; } = null!;
    public DbSet<NewsSummary> NewsSummaries { get; set; } = null!;

    public string DbPath { get; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }
}