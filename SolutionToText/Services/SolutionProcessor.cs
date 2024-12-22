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
    internal string Process(DirectoryInfo rootPath)
    {
        var filesStruct = new FileStructureCollector();
        var filesCollector = new SourceFileCollector([".cs", ".js", ".css"]);

        var directoryTraverser = new DirectoryWalker(filesStruct, filesCollector);
        directoryTraverser.WalkDirectory(rootPath);

        var destinationFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "result.txt");

        using var fileWriter = new ContentWriter(destinationFilePath);

        var buffer = new char[2048];

        fileWriter.WriteFilesStructure(filesStruct);

        foreach (var file in filesCollector.GetSourceFiles())
        {
            fileWriter.WriteFileContent(file, buffer, rootPath.FullName);
        }

        return destinationFilePath;
    }
}