using System.Text;
using SolutionToText.Interfaces;

namespace SolutionToText.Services;

/// <summary>
/// Provides methods for collect files structure information and return it in string format.
/// </summary>
internal sealed class FileStructureCollector : IFileStructureCollector
{
    private const char TabSymbol = '-';
    private readonly StringBuilder _filesStructure = new();

    /// <inheritdoc />
    public void AddFile(FileInfo file, int depth)
    {
        _filesStructure.AppendLine($"{new string(TabSymbol, depth)} {file.Name}");
    }

    /// <inheritdoc />
    public void AddDirectory(DirectoryInfo directory, int depth)
    {
        _filesStructure.AppendLine($"{new string(TabSymbol, depth)} {directory.Name}");
    }
    
    /// <inheritdoc />
    public string GetFilesStructure() => _filesStructure.ToString();
}
