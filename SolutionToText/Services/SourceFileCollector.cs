using SolutionToText.Interfaces;

namespace SolutionToText.Services;

/// <summary>
/// Provides methods for collecting files and returning them as a list.
/// </summary>
internal sealed class SourceFileCollector : ISourceFileCollector
{
    private readonly HashSet<string> _includeExtensions;
    private readonly List<FileInfo> _files = [];

    public SourceFileCollector(IEnumerable<string> includeExtensions)
    {
        _includeExtensions = includeExtensions.ToHashSet();
    }

    /// <inheritdoc />
    public void AddFileSource(FileInfo file)
    {
        if (!_includeExtensions.Contains(file.Extension))
            return;

        _files.Add(file);
    }

    /// <inheritdoc />
    public List<FileInfo> GetSourceFiles() => _files;
}
