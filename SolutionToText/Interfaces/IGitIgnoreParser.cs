using System.Text.RegularExpressions;

namespace SolutionToText.Interfaces;

/// <summary>
/// Defines methods for read .gitignore.
/// </summary>
internal interface IGitIgnoreParser
{
    /// <summary>
    /// Reads the .gitignore file and returns a list of precompiled regular expressions.
    /// </summary>
    /// <param name="gitIgnoreFile">Information about the .gitignore file.</param>
    /// <returns>A list of regular expressions for ignoring.</returns>
    List<Regex> ParseGitignoreFile(FileInfo? gitIgnoreFile);
}