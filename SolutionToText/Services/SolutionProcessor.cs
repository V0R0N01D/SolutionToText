using System.Buffers;

namespace SolutionToText.Services;

/// <summary>
/// Класс для обработки файлов решения, объединяющий их содержимое в один файл.
/// </summary>
class SolutionProcessor
{
	/// <summary>
	/// Обрабатывает все файлы в указанной директории, 
	/// применяет фильтрацию на основе .gitignore и объединяет содержимое файлов в один текстовый файл.
	/// </summary>
	/// <param name="rootPath"></param>
	/// <returns></returns>
	internal string Process(string rootPath)
	{
		var collector = new FileCollector();
		var filesPath = collector.Collect(rootPath);

		var destinationFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "result.txt");

		using var destinationStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write);
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
