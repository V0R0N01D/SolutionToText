namespace SolutionToText.Services;

/// <summary>
/// Processes solution files and combines their contents into a single output file.
/// </summary>
internal sealed class SolutionProcessor
{
    /// <summary>
    /// Combines filtered solution files into single output file
    /// </summary>
    /// <param name="rootPath">Solution root directory.</param>
    /// <returns>Path to the generated output file.</returns>
    internal string Process(DirectoryInfo rootPath)
    {
        var filesStruct = new FileStructureCollector();
        var filesCollector =
            new SourceFileCollector([".cs", ".js", ".css", ".cshtml", ".cshtml.cs"]);
        var gitIgnoreParser = new GitIgnoreParser();

        var directoryWalker = new DirectoryWalker(filesStruct, filesCollector, gitIgnoreParser);
        directoryWalker.WalkDirectory(rootPath);

        var destinationFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "result.txt");

        using var fileWriter = new ContentWriter(destinationFilePath);

        fileWriter.WriteFilesStructure(filesStruct);

        foreach (var file in filesCollector.GetSourceFiles())
        {
            fileWriter.WriteFileContent(file, rootPath.FullName);
        }

        return destinationFilePath;
    }
}