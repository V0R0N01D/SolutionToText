using System.Text.RegularExpressions;
using SolutionToText.Interfaces;

namespace SolutionToText.Services;

/// <summary>
/// A class that recursively walking directories and passes data to other classes.
/// </summary>
internal sealed class DirectoryWalker : IDirectoryWalker
{
    private readonly IFileStructureCollector _fileMapCollector;
    private readonly ISourceFileCollector _fileCollector;

    /// <summary>
    /// Stack of precompiled regex patterns for file/directory exclusion.
    /// </summary>
    private readonly Stack<List<Regex>> _excludePatternsStack = new();

    internal DirectoryWalker(IFileStructureCollector fileMapCollector,
        ISourceFileCollector fileCollector)
    {
        _fileCollector = fileCollector;
        _fileMapCollector = fileMapCollector;
        
        var initialPatterns = new List<Regex>
        {
            new Regex(@"^obj$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"^.git$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"^bin$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"^wwwroot$", RegexOptions.Compiled | RegexOptions.IgnoreCase),            new Regex(@"^bin$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"^.idea$", RegexOptions.Compiled | RegexOptions.IgnoreCase),            new Regex(@"^bin$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"^.vs$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        };
        _excludePatternsStack.Push(initialPatterns);
    }

    /// <summary>
    /// Processes directory tree starting from root path,
    /// collecting files matching specified extensions.
    /// </summary>
    /// <param name="rootPath">Root directory to start processing from.</param>
    public void WalkDirectory(DirectoryInfo rootPath)
    {
        ProcessDirectory(rootPath, 1);
    }

    /// <summary>
    /// Recursively walking files
    /// from the current directory and its subdirectories,
    /// applying current exclusion patterns for filtering.
    /// </summary>
    /// <param name="currentDirectory">The current directory.</param>
    /// <param name="depth">The current depth level.</param>
    private void ProcessDirectory(DirectoryInfo currentDirectory, int depth)
    {
        CheckGitIgnore(currentDirectory);

        // Process subdirectories.
        foreach (var directory in currentDirectory.GetDirectories())
        {
            if (IsExcluded(directory.Name, true))
                continue;

            _fileMapCollector.AddDirectory(directory, depth);

            ProcessDirectory(directory, depth + 1);
        }

        // Process files in the current directory.
        foreach (var file in currentDirectory.GetFiles())
        {
            _fileMapCollector.AddFile(file, depth);

            if (!IsExcluded(file.Name, false))
                _fileCollector.AddFileSource(file);
        }

        // Remove the current directory's patterns from the stack.
        _excludePatternsStack.Pop();
    }

    /// <summary>
    /// Determines whether a file or directory should be excluded
    /// based on the current stack of exclusion patterns.
    /// </summary>
    /// <param name="name">The name of the file or directory.</param>
    /// <param name="isDirectory">Indicates whether the object is a directory.</param>
    /// <returns>True if the file or directory should be excluded, otherwise False.</returns>
    private bool IsExcluded(string name, bool isDirectory)
    {
        foreach (var patterns in _excludePatternsStack)
        {
            foreach (var regex in patterns)
            {
                if (regex.IsMatch(name))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks for the existence of .gitignore and adds its rules to the exclusions.
    /// </summary>
    /// <param name="currentDirectory">The directory in which the file's
    /// presence is checked.</param>
    private void CheckGitIgnore(DirectoryInfo currentDirectory)
    {
        var gitIgnoreFile = currentDirectory.GetFiles(".gitignore").FirstOrDefault();
        _excludePatternsStack.Push(ParseGitignoreFile(gitIgnoreFile));
    }

    /// <summary>
    /// Reads the .gitignore file and returns a list of precompiled regular expressions.
    /// </summary>
    /// <param name="gitIgnoreFile">Information about the .gitignore file.</param>
    /// <returns>A list of regular expressions for ignoring.</returns>
    private List<Regex> ParseGitignoreFile(FileInfo? gitIgnoreFile)
    {
        if (gitIgnoreFile == null || !gitIgnoreFile.Exists)
            return new();

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
            
            var isDirectoryPattern = trimmedLine.EndsWith("/");

            string pattern = trimmedLine.TrimEnd('/');

            // Convert pattern in regular expression.
            string regexPattern = "^" + Regex.Escape(pattern)
                                        .Replace("\\*", ".*")
                                        .Replace("\\?", ".") + "$";

            var options = RegexOptions.Compiled | RegexOptions.IgnoreCase;

            // Если это паттерн для директории, добавляем проверку на конец строки
            if (isDirectoryPattern)
                regexPattern += @"(\\|/)?$";

            patterns.Add(new Regex(regexPattern, options));
        }

        return patterns;
    }
}
