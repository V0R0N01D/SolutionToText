using System.Text.RegularExpressions;

namespace SolutionToText.Interfaces;

/// <summary>
/// Defines methods for parsing .gitignore.
/// </summary>
internal interface IGitIgnoreParser
{
    /// <summary>
    /// Parses the .gitignore file and returns a list of precompiled regular expressions.
    /// </summary>
    /// <param name="gitIgnoreFile">Information about the .gitignore file.</param>
    /// <returns>A list of regular expressions for ignoring.</returns>
    List<Regex> ParseGitignoreFile(FileInfo? gitIgnoreFile);
}