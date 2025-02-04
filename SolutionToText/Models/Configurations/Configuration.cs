namespace SolutionToText.Models.Configurations;

/// <summary>
/// Represents a configuration that defines settings for processing the solution.
/// </summary>
public class Configuration
{
    /// <summary>
    /// Gets the title of the configuration.
    /// </summary>
    public required string Title { get; set; }
    
    /// <summary>
    /// Gets the collection of file extensions to include in the processing. 
    /// </summary>
    public required IEnumerable<string> IncludeFileExtensions { get; set; }
    
    /// <summary>
    /// Gets the collection of patterns used to exclude files or directories.
    /// </summary>
    public required IEnumerable<string> ExcludePatterns { get; set; }
}