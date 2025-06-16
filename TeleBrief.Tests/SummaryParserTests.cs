using Xunit;

namespace TeleBrief.Tests;

public class SummaryParserTests
{
    [Fact]
    public void Parse_ReturnsCategoriesWithFacts()
    {
        var input = @"Category One:
- Fact 1.
- Fact 2.
Second Category:
- Item A.
- Item B.";

        var result = SummaryParser.Parse(input);

        Assert.Equal(2, result.Count);
        Assert.Equal(new[]{"Fact 1.", "Fact 2."}, result["Category One"]);
        Assert.Equal(new[]{"Item A.", "Item B."}, result["Second Category"]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_EmptyOrWhitespace_ReturnsEmpty(string? input)
    {
        var result = SummaryParser.Parse(input);
        Assert.Empty(result);
    }
}
