using SolutionToText.Services;

namespace SolutionToText;

class Program
{
	static void Main(string[] args)
	{
		try
		{
			Console.WriteLine("Введите путь к папке с решением:");
			var rootPath = Console.ReadLine();

			if (!Directory.Exists(rootPath))
			{
				Console.WriteLine("Указанная папка не существует.");
				return;
			}

			var destinationFilePath = SolutionProcessor.Process(rootPath);

			Console.WriteLine($"Обработка завершена. Объединенный файл создан: {destinationFilePath}.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Произошла ошибка: {ex.Message}");
		}

		Console.ReadLine();
	}
}
