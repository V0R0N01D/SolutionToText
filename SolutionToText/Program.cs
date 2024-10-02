using SolutionToText.Services;

namespace SolutionToText;

internal class Program
{
	static void Main(string[] args)
	{
		try
		{
			Console.WriteLine("Введите путь к папке с решением:");
			var solutionPath = Console.ReadLine();

			if (!Directory.Exists(solutionPath))
			{
				Console.WriteLine("Указанная папка не существует.");
				return;
			}

			var processor = new SolutionProcessor();
			processor.Process(solutionPath);

			

			Console.WriteLine("Обработка завершена. Объединенный файл создан.");

			
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Произошла ошибка: {ex.Message}");
		}
	}
}
