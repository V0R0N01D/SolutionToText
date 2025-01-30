namespace SolutionToText.Interfaces;

/// <summary>
/// Defines a method for getting the root directory.
/// </summary>
internal interface IPathService
{
    /// <summary>
    /// Getting the root directory as an object <see cref="DirectoryInfo"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="DirectoryInfo"/> object representing the root directory.
    /// </returns>
    DirectoryInfo GetRootDirectory();
}