using System.Buffers;

namespace SolutionToText.Services;

/// <summary>
/// Класс для обработки файлов решения, объединяющий их содержимое в один файл на рабочем столе.
/// </summary>
class SolutionProcessor
{
	/// <summary>
	/// Обрабатывает все файлы с нужными расширениями в указанной директории и её подпапках,
	/// применяет фильтрацию на основе .gitignore и объединяет их содержимое 
	/// в один текстовый файл 'result.txt' на рабочем столе пользователя.
	/// </summary>
	/// <param name="rootPath">Путь к корневой директории решения.</param>
	/// <returns>Путь к созданному объединенному файлу.</returns>
	internal static string Process(string rootPath)
	{
		var collector = new FileCollector();
		var filesPath = collector.Collect(rootPath);

		var destinationFilePath = 
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "result.txt");

		using var destinationStream = 
			new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write);
		using var writer = new StreamWriter(destinationStream);

		var buffer = ArrayPool<char>.Shared.Rent(2048);

		try
		{
			foreach (var file in filesPath)
			{
				writer.WriteLine($"Содержимое файла {file.Replace(rootPath, string.Empty)}:");
				buffer.CopyFileContent(file, writer);

				writer.WriteLine("\n");
			}
		}
		finally
		{
			ArrayPool<char>.Shared.Return(buffer);
		}

		return destinationFilePath;
	}
}
