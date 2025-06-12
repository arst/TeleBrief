namespace TeleBrief;

public static class SummaryParser
{
    public static Dictionary<string, List<string>> Parse(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return new Dictionary<string, List<string>>();

        var sections = new Dictionary<string, List<string>>();
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        string? currentCategory = null;

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            if (line.EndsWith(':'))
            {
                currentCategory = line.TrimEnd(':').Trim();
                sections[currentCategory] = new List<string>();
            }
            else if (line.StartsWith("-") && currentCategory != null)
            {
                var fact = line.TrimStart('-', ' ').Trim();
                if (!string.IsNullOrWhiteSpace(fact))
                    sections[currentCategory].Add(fact);
            }
        }

        return sections;
    }
}
