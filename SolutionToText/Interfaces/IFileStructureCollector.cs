namespace SolutionToText.Interfaces;

/// <summary>
/// Defines methods for collecting and managing file structure information.
/// </summary>
internal interface IFileStructureCollector
{
    /// <summary>
    /// Adds a file to the structure collection with the specified indentation.
    /// </summary>
    /// <param name="file">The file information to add.</param>
    /// <param name="depth">The current depth level.</param>
    void AddFile(FileInfo file, int depth);

    /// <summary>
    /// Adds a directory to the structure collection with the specified indentation.
    /// </summary>
    /// <param name="directory">The directory information to add.</param>
    /// <param name="depth">The current depth level.</param>
    void AddDirectory(DirectoryInfo directory, int depth);

    /// <summary>
    /// Retrieves the collected file structure as a formatted string.
    /// </summary>
    /// <returns>A string representation of the file structure.</returns>
    string GetFilesStructure();
}
