using SolutionToText.Interfaces;

namespace SolutionToText.Services;

/// <summary>
/// Класс, рекурсивно собирающий файлы с указанными расширениями из директорий и их поддиректорий,
/// применяя фильтрацию на основе файлов .gitignore.
/// </summary>
class SourceFileCollector : ISourceFileCollector
{
    /// <summary>
    /// Набор расширений файлов, которые необходимо включить в результат.
    /// </summary>
    private readonly HashSet<string> _includeExtensions;

    private readonly List<FileInfo> _files = new();

    public SourceFileCollector(IEnumerable<string> includeExtensions)
    {
        _includeExtensions = includeExtensions.ToHashSet();
    }

    public void AddFileSource(FileInfo file)
    {
        // фильтрация по расширению
        if (!_includeExtensions.Contains(file.Extension))
            return;

        _files.Add(file);
    }

    public List<FileInfo> GetSourceFiles() => _files;
}
