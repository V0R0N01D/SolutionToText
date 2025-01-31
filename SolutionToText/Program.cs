using SolutionToText.Services;

namespace SolutionToText;

class Program
{
    static void Main(string[] args)
    {
        var pathValidator = new PathValidator();
        var pathService = new ConsolePathService(pathValidator);
        var fileStructureCollector = new FileStructureCollector();
        var sourceFileCollector =
            new SourceFileCollector([".cs", ".js", ".css", ".cshtml", ".cshtml.cs"]);
        var gitIgnoreParser = new GitIgnoreParser();
        
        var destinationFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "result.txt");

        var solutionProcessor =
            new SolutionProcessor(pathService, fileStructureCollector, sourceFileCollector, gitIgnoreParser);
        
        try
        {
            solutionProcessor.ConvertSolutionToText(destinationFilePath);

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