using SolutionToText.Services;

namespace SolutionToText;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Enter the path to the solution folder:");
            var rootPath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(rootPath))
            {
                Console.WriteLine("Empty field provided.");
                return;
            }

            var directoryInfo = new DirectoryInfo(rootPath);
            if (!directoryInfo.Exists)
            {
                Console.WriteLine("The specified folder does not exist.");
                return;
            }

            var solutionProcessor = new SolutionProcessor();
            var destinationFilePath = solutionProcessor.Process(directoryInfo);

            Console.WriteLine($"Processing completed. Combined file created: {destinationFilePath}.");

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = destinationFilePath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        Console.ReadLine();
    }
}