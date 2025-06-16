using Microsoft.EntityFrameworkCore;

namespace TeleBrief.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<TopicState> TopicStates { get; set; } = null!;
    public DbSet<NewsSummary> NewsSummaries { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={Db.Path}");
    }
}