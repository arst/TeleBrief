using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace TeleBrief.Infrastructure.Plugins;

public class DatePlugin
{
    [KernelFunction, Description("Returns the current date in UTC in the format yyyy-MM-dd")]
    public string GetTodayDate()
    {
        return DateTime.UtcNow.ToString("yyyy-MM-dd");
    }
}