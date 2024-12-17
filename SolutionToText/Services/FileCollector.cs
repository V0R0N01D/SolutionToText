using System.Text.RegularExpressions;

namespace SolutionToText.Services;

/// <summary>
/// Класс, рекурсивно собирающий файлы с указанными расширениями из директорий и их поддиректорий,
/// применяя фильтрацию на основе файлов .gitignore.
/// </summary>
internal class FileCollector
{
    /// <summary>
    /// Набор расширений файлов, которые необходимо включить в список результат.
    /// </summary>
    private readonly HashSet<string> _includeExtensions = [".cs", ".js", ".css"];

    private readonly List<FileInfo> _files = new List<FileInfo>();

    internal void Add(FileInfo file)
    {
        // фильтрация по расширению
        if (!_includeExtensions.Contains(file.Extension))
            return;

        _files.Add(file);
    }

    internal List<FileInfo> GetFiles() => _files;
}
