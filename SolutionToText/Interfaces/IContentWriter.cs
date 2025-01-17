namespace SolutionToText.Interfaces;

/// <summary>
/// Defines methods for writing file content and structure to an output destination.
/// </summary>
internal interface IContentWriter
{
    /// <summary>
    /// Writes the file structure information using the provided collector.
    /// </summary>
    /// <param name="filesStructureCollector">The collector containing file
    /// structure information.</param>
    void WriteFilesStructure(IFileStructureCollector filesStructureCollector);

    /// <summary>
    /// Writes the content of a specific file to the output destination.
    /// </summary>
    /// <param name="file">The file information to write.</param>
    /// <param name="buffer">The buffer used for reading file content.</param>
    /// <param name="rootPath">The root path of the solution.</param>
    void WriteFileContent(FileInfo file, char[] buffer, string rootPath);
}
