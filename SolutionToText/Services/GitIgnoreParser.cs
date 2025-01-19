using System.Text.RegularExpressions;
using SolutionToText.Interfaces;

namespace SolutionToText.Services;

internal sealed class GitIgnoreParser : IGitIgnoreParser
{
    /// <inheritdoc />
    public List<Regex> ParseGitignoreFile(FileInfo? gitIgnoreFile)
    {
        if (gitIgnoreFile == null || !gitIgnoreFile.Exists)
            return [];

        var patterns = new List<Regex>();
        var lines = File.ReadAllLines(gitIgnoreFile.FullName);

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrEmpty(trimmedLine))
                continue;

            // Ignore comment.
            if (trimmedLine.StartsWith("#"))
                continue;

            // Ignore negative pattern.
            if (trimmedLine.StartsWith("!"))
                continue;

            var isDirectoryPattern = trimmedLine.EndsWith('/');

            string pattern = trimmedLine.TrimEnd('/');

            // Convert pattern in regular expression.
            string regexPattern = "^" + Regex.Escape(pattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") + "$";

            var options = RegexOptions.Compiled | RegexOptions.IgnoreCase;
            
            if (isDirectoryPattern)
                regexPattern += @"(\\|/)?$";

            patterns.Add(new Regex(regexPattern, options));
        }

        return patterns;
    }
}