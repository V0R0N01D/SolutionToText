namespace SolutionToText.Models.Configurations;

/// <summary>
/// Represents the settings used for processing the solution.
/// </summary>
public class Settings
{
    /// <summary>
    /// Gets the title of the selected configuration.
    /// </summary>
    public required string SelectedConfigurationTitle { get; set; }
}