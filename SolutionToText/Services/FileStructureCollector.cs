using System.Text;
using SolutionToText.Interfaces;

namespace SolutionToText.Services;

/// <summary>
/// Collects and maintains project file structure information
/// </summary>
class FileStructureCollector : IFileStructureCollector
{
    private const char TabSymbol = '-';
    private readonly StringBuilder _filesStructure = new();

    public void AddFile(FileInfo file, int depth)
    {
        _filesStructure.AppendLine($"{new string(TabSymbol, depth)} {file.Name}");
    }

    public void AddDirectory(DirectoryInfo directory, int depth)
    {
        _filesStructure.AppendLine($"{new string(TabSymbol, depth)} {directory.Name}");
    }
    
    public string GetFilesStructure() => _filesStructure.ToString();
}
