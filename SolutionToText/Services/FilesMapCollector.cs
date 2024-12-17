using System.Text;

namespace SolutionToText.Services;

/// <summary>
/// Класс который используется, чтобы создать карту файлов в проекте.
/// </summary>
internal class FilesMapCollector
{
    private readonly StringBuilder _filesMap = new StringBuilder();

    internal void AddFile(FileInfo file, string currentTab)
    {
        _filesMap.AppendLine($"{currentTab} {file.Name}");
    }

    internal void AddDirectory(DirectoryInfo directory, string currentTab)
    {
        _filesMap.AppendLine($"{currentTab} {directory.Name}");
    }

    internal string GetFilesMap() => _filesMap.ToString();
}
