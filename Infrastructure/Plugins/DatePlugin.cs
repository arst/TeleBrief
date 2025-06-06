using Microsoft.SemanticKernel;

namespace TeleBrief.Infrastructure.Plugins;

public class DatePlugin
{
    [KernelFunction]
    public string GetTodayDate()
    {
        return DateTime.UtcNow.ToString("yyyy-MM-dd");
    }
}