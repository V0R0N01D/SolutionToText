namespace SolutionToText.Interfaces;

/// <summary>
/// Defines methods for collecting source files.
/// </summary>
internal interface ISourceFileCollector
{
    /// <summary>
    /// Adds a file information to the collection.
    /// </summary>
    /// <param name="file">The file information.</param>
    void AddFileSource(FileInfo file);

    /// <summary>
    /// Retrieves the list of file information.
    /// </summary>
    /// <returns>A list of collected file information.</returns>
    List<FileInfo> GetSourceFiles();
}
