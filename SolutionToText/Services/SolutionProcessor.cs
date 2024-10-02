namespace SolutionToText.Services;

/// <summary>
/// Класс для управления процессом 
/// </summary>
internal class SolutionProcessor
{
	public void Process(string solutionPath)
	{
		var collector = new FileCollector();
		var files = collector.Collect(solutionPath);

		var reader = new FileReader();
		var aggregator = new ContentAggregator();

		foreach (var file in files)
		{
			Console.WriteLine(file);
		}
	}
}
