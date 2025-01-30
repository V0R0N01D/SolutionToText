using SolutionToText.Interfaces;
using SolutionToText.Models;

namespace SolutionToText.Services;

/// <summary>
/// Provides method for validate string representing a directory path.
/// </summary>
public class PathValidator : IPathValidator
{
    /// <inheritdoc />
    public Result<DirectoryInfo?> ValidatePath(string? directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
            return Result<DirectoryInfo?>.Failure("Directory path cannot be empty.");

        var directoryInfo = new DirectoryInfo(directoryPath);
        if (!directoryInfo.Exists)
            return Result<DirectoryInfo?>.Failure("Directory does not exist.");

        if (directoryInfo.GetFiles().Length == 0 && directoryInfo.GetDirectories().Length == 0)
            return Result<DirectoryInfo?>
                .Failure("Directory does not contain files and subdirectories.");
        
        return Result<DirectoryInfo?>.Success(directoryInfo);
    }
}