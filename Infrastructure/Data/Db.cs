namespace TeleBrief.Infrastructure.Data;

public static class Db
{
    public static string Path { get; } = System.IO.Path.Join(Directory.GetCurrentDirectory(), "telebrief.db");

    public static string ConnectionString { get; } = $"Data Source={Path}";
}