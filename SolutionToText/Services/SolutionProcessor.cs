using System.Text.RegularExpressions;
using SolutionToText.Interfaces;

namespace SolutionToText.Services;

/// <summary>
/// Provides method for processes solution files
/// and combines their contents into a single output file.
/// </summary>
internal sealed class SolutionProcessor
{
    private readonly IPathService _pathService;
    private readonly IFileStructureCollector _fileStructureCollector;
    private readonly ISourceFileCollector _sourceFileCollector;
    private readonly IGitIgnoreParser _gitIgnoreParser;
    private readonly IEnumerable<Regex> _initialExcludePatterns;

    public SolutionProcessor(
        IPathService pathService,
        IFileStructureCollector fileStructureCollector,
        ISourceFileCollector sourceFileCollector,
        IGitIgnoreParser gitIgnoreParser,
        IEnumerable<Regex> initialExcludePatterns)
    {
        _pathService = pathService;
        _fileStructureCollector = fileStructureCollector;
        _sourceFileCollector = sourceFileCollector;
        _gitIgnoreParser = gitIgnoreParser;
        _initialExcludePatterns = initialExcludePatterns;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="destinationFilePath">The path to the file that needs to be created.</param>
    internal void ConvertSolutionToText(string destinationFilePath)
    {
        var rootDirectory = _pathService.GetRootDirectory();

        var directoryWalker = new DirectoryWalker(_fileStructureCollector, _sourceFileCollector,
            _gitIgnoreParser, _initialExcludePatterns);
        directoryWalker.WalkDirectory(rootDirectory);

        using var fileWriter = new ContentWriter(destinationFilePath);
        fileWriter.WriteFilesStructure(_fileStructureCollector);

        foreach (var file in _sourceFileCollector.GetSourceFiles())
        {
            fileWriter.WriteFileContent(file, rootDirectory.FullName);
        }
    }
}