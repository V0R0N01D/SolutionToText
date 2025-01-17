using System.Text;
using SolutionToText.Interfaces;

namespace SolutionToText.Services;

/// <summary>
/// Collects and maintains project file structure information
/// </summary>
class FileStructureCollector : IFileStructureCollector
{
    private readonly StringBuilder _filesStructure = new();

    public void AddFile(FileInfo file, string currentTab)
    {
        _filesStructure.AppendLine($"{currentTab} {file.Name}");
    }

    public void AddDirectory(DirectoryInfo directory, string currentTab)
    {
        _filesStructure.AppendLine($"{currentTab} {directory.Name}");
    }

    public string GetFilesStructure() => _filesStructure.ToString();
}
