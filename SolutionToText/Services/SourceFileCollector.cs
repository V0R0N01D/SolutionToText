using SolutionToText.Interfaces;

namespace SolutionToText.Services;

/// <summary>
/// A class that recursively collects files with specified extensions from directories 
/// and their subdirectories, applying filtering based on .gitignore files.
/// </summary>
internal sealed class SourceFileCollector : ISourceFileCollector
{
    private readonly HashSet<string> _includeExtensions;
    private readonly List<FileInfo> _files = [];

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
