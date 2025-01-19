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
    private readonly IGitIgnoreParser _gitIgnoreParser;

    /// <summary>
    /// Stack of precompiled regex patterns for file/directory exclusion.
    /// </summary>
    private readonly Stack<List<Regex>> _excludePatternsStack = new();

    internal DirectoryWalker(IFileStructureCollector fileMapCollector,
        ISourceFileCollector fileCollector,
        IGitIgnoreParser gitIgnoreParser)
    {
        _fileCollector = fileCollector;
        _fileMapCollector = fileMapCollector;
        _gitIgnoreParser = gitIgnoreParser;
        
        var initialPatterns = new List<Regex>
        {
            new Regex(@"^obj$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"^.git$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"^bin$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"^wwwroot$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"^.idea$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
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
        TryAddGitIgnore(currentDirectory);

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
    private void TryAddGitIgnore(DirectoryInfo currentDirectory)
    {
        var gitIgnoreFile = currentDirectory.GetFiles(".gitignore").FirstOrDefault();
        var ignoreRules = _gitIgnoreParser.ParseGitignoreFile(gitIgnoreFile);
        _excludePatternsStack.Push(ignoreRules);
    }
}
