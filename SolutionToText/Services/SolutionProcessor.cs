using System.Buffers;

namespace SolutionToText.Services;

/// <summary>
/// Класс для обработки файлов решения, объединяющий их содержимое в один файл на рабочем столе.
/// </summary>
internal class SolutionProcessor
{
    /// <summary>
    /// Обрабатывает все файлы с нужными расширениями в указанной директории и её подпапках,
    /// применяет фильтрацию на основе .gitignore и объединяет их содержимое 
    /// в один текстовый файл 'result.txt' на рабочем столе пользователя.
    /// </summary>
    /// <param name="rootPath">Путь к корневой директории решения.</param>
    /// <returns>Путь к созданному объединенному файлу.</returns>
    internal static string Process(DirectoryInfo rootPath)
    {
        var filesMap = new FilesMapCollector();
        var filesCollector = new FileCollector();

        var directoryTraverser = new DirectoryTraverser(filesMap, filesCollector);
        directoryTraverser.TraverseDirectory(rootPath);

        var destinationFilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "result.txt");

        using var destinationStream =
            new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write);
        using var writer = new StreamWriter(destinationStream);

        var buffer = ArrayPool<char>.Shared.Rent(2048);

        try
        {
            writer.WriteLine(filesMap.GetFilesMap());

            foreach (var file in filesCollector.GetFiles())
            {
                writer.WriteLine($"Содержимое файла {file
                    .FullName.Replace(rootPath.FullName, string.Empty)}:");
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
