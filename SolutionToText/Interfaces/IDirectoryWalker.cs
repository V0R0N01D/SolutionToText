namespace SolutionToText.Interfaces;

/// <summary>
/// Defines methods for walking directory structures.
/// </summary>
internal interface IDirectoryWalker
{
    /// <summary>
    /// Walks through a directory structure starting from the specified root path.
    /// </summary>
    /// <param name="rootPath">The starting directory.</param>
    void WalkDirectory(DirectoryInfo rootPath);
}
