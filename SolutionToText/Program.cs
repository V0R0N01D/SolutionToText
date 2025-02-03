using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using SolutionToText.Models;
using SolutionToText.Services;

namespace SolutionToText;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var configurations = configuration.GetSection("Configurations")
                .Get<IEnumerable<Configuration>>();
            var currentConfiguration = configurations!.First();

            var pathValidator = new PathValidator();
            var pathService = new ConsolePathService(pathValidator);
            var fileStructureCollector = new FileStructureCollector();
            var sourceFileCollector =
                new SourceFileCollector(currentConfiguration.IncludeFileExtensions);
            var gitIgnoreParser = new GitIgnoreParser();

            var destinationFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "result.txt");

            var solutionProcessor =
                new SolutionProcessor(pathService, fileStructureCollector,
                    sourceFileCollector, gitIgnoreParser,
                    currentConfiguration.ExcludePatterns
                        .Select(pattern =>
                            new Regex($"^{pattern}$",
                                RegexOptions.Compiled | RegexOptions.IgnoreCase))
                        .ToArray());

            solutionProcessor.ConvertSolutionToText(destinationFilePath);

            Console.WriteLine($"Processing completed. Combined file created: {destinationFilePath}.");

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = destinationFilePath,
                UseShellExecute = true
            });
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Error: Configuration file (appsettings.json) not found.");
        }
        catch (ArgumentNullException)
        {
            Console.WriteLine("Error: Configuration file is corrupted or has an invalid format.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred while processing the solution: {ex.Message}");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}