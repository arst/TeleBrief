using TeleBrief.Topics;
using Xunit;

namespace TeleBrief.Tests;

public class TopicAnalysisServiceTests
{
    [Theory]
    [InlineData("State: 75", 50, 75)]
    [InlineData("75", 50, 75)]
    [InlineData("State: not-a-number", 10, 10)]
    [InlineData("", 10, 10)]
    [InlineData(null, 20, 20)]
    public void ParseStateLine_ReturnsExpectedValue(string? input, int fallback, int expected)
    {
        var result = TopicAnalysisService.ParseStateLine(input, fallback);
        Assert.Equal(expected, result);
    }
}
