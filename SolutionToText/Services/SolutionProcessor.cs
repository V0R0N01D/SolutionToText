namespace SolutionToText.Services;

/// <summary>
/// Processes solution files and combines their contents into a single output file.
/// </summary>
class SolutionProcessor
{
    /// <summary>
    /// Combines filtered solution files into single output file
    /// </summary>
    /// <param name="rootPath">Solution root directory.</param>
    /// <returns>Path to the generated output file.</returns>
    internal string Process(DirectoryInfo rootPath)
    {
        var filesStruct = new FileStructureCollector();
        var filesCollector = new SourceFileCollector([".cs", ".js", ".css", ".cshtml", ".cshtml.cs"]);

        var directoryWalker = new DirectoryWalker(filesStruct, filesCollector);
        directoryWalker.WalkDirectory(rootPath);

        var destinationFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "result.txt");

        using var fileWriter = new ContentWriter(destinationFilePath);

        var buffer = new char[2048];

        fileWriter.WriteFilesStructure(filesStruct);

        foreach (var file in filesCollector.GetSourceFiles())
        {
            fileWriter.WriteFileContent(file, buffer, rootPath.FullName);
        }

        return destinationFilePath;
    }
}