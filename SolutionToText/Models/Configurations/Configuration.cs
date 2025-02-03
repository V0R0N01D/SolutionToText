namespace SolutionToText.Models.Configurations;

public class Configuration
{
    public required string Title { get; set; }
    public required IEnumerable<string> IncludeFileExtensions { get; set; }
    public required IEnumerable<string> ExcludePatterns { get; set; }
}