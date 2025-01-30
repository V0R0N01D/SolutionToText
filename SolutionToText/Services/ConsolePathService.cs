using SolutionToText.Interfaces;

namespace SolutionToText.Services;

/// <summary>
/// Provides method for getting the root directory from the console.
/// </summary>
internal sealed class ConsolePathService : IPathService
{
    private readonly IPathValidator _pathValidator;

    public ConsolePathService(IPathValidator pathValidator)
    {
        _pathValidator = pathValidator;
    }
    
    /// <inheritdoc />
    public DirectoryInfo GetRootDirectory()
    {
        while (true)
        {
            Console.WriteLine("Enter path to solution folder (or Ctrl+C to exit):");
            var input = Console.ReadLine()?.Trim();

            var validateResult = _pathValidator.ValidatePath(input);

            if (validateResult.IsSuccess)
                return validateResult.Value!;
            
            HandleError(validateResult.Message!);
        }
    }

    private static void HandleError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
        Console.WriteLine("Try again...\n");
    }
}