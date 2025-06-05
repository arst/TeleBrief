namespace TeleBrief.Infrastructure.Gemini;

public class GeminiResponse
{
    public required List<Candidate> Candidates { get; set; }

    public class Candidate
    {
        public required Content Content { get; set; }
    }

    public class Content
    {
        public required List<Part> Parts { get; set; }
    }

    public class Part
    {
        public required string Text { get; set; }
    }
}