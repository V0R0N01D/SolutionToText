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

			if (string.IsNullOrWhiteSpace(rootPath))
			{
                Console.WriteLine("Передано пустое поле.");
                return;
            }

			var directoryInfo = new DirectoryInfo(rootPath);

			if (!directoryInfo.Exists)
			{
				Console.WriteLine("Указанная папка не существует.");
				return;
			}

			var destinationFilePath = SolutionProcessor.Process(directoryInfo);

			Console.WriteLine($"Обработка завершена. Объединенный файл создан: {destinationFilePath}.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Произошла ошибка: {ex.Message}");
		}

		Console.ReadLine();
	}
}
