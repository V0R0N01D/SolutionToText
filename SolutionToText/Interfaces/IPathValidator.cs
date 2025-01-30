using SolutionToText.Models;

namespace SolutionToText.Interfaces;

/// <summary>
/// Defines a method for validating string with directory path and returning a result.
/// </summary>
internal interface IPathValidator
{
    /// <summary>
    /// Validates a string representing a directory path.
    /// </summary>
    /// <param name="directoryPath">The directory path to validate.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> object:
    /// - If the path is valid, it contains a <see cref="DirectoryInfo"/> instance.
    /// - If the path is invalid, it contains an error describing the issue.
    /// </returns>
    Result<DirectoryInfo?> ValidatePath(string? directoryPath);
}