using SolutionToText.Interfaces;

namespace SolutionToText.Services;

/// <summary>
/// A class that recursively collects files with specified extensions from directories 
/// and their subdirectories, applying filtering based on .gitignore files.
/// </summary>
class SourceFileCollector : ISourceFileCollector
{
    /// <summary>
    /// A set of file extensions that need to be included in the result.
    /// </summary>
    private readonly HashSet<string> _includeExtensions;

    private readonly List<FileInfo> _files = new();

    public SourceFileCollector(IEnumerable<string> includeExtensions)
    {
        _includeExtensions = includeExtensions.ToHashSet();
    }

    public void AddFileSource(FileInfo file)
    {
        if (!_includeExtensions.Contains(file.Extension))
            return;

        _files.Add(file);
    }

    public List<FileInfo> GetSourceFiles() => _files;
}
