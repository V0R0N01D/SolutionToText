using System.Text.RegularExpressions;

namespace SolutionToText.Services;

/// <summary>
/// Класс который рекурсивно проходится по директориям и передает данные в другие классы.
/// </summary>
internal class DirectoryTraverser
{
    private const char TabSimbol = '-';

    private readonly FilesMapCollector _fileMapCollector;
    private readonly FileCollector _fileCollector;

    /// <summary>
    /// Стек списков предкомпилированных регулярных выражений
    /// для исключения файлов и директорий.
    /// </summary>
    private Stack<List<Regex>> _excludePatternsStack = new();

    internal DirectoryTraverser(FilesMapCollector fileMapCollector,
        FileCollector fileCollector)
    {
        _fileCollector = fileCollector;
        _fileMapCollector = fileMapCollector;

        // Предварительное исключение папки "obj"
        var initialPatterns = new List<Regex>
        {
            new Regex(@"^obj$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"^.git$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"^bin$", RegexOptions.Compiled | RegexOptions.IgnoreCase)
        };
        _excludePatternsStack.Push(initialPatterns);
    }

    /// <summary>
    /// Собирает файлы с расширениями из <see cref="_includeExtensions"/>, 
    /// начиная с указанной директории и её подпапок,
    /// применяя фильтрацию на основе шаблонов исключений из файлов .gitignore.
    /// </summary>
    /// <param name="rootPath">Путь к корневой директории,
    /// с которой начинается сбор файлов.</param>
    internal void TraverseDirectory(DirectoryInfo rootPath)
    {
        ProcessDirectory(rootPath, TabSimbol.ToString());
    }

    /// <summary>
    /// Рекурсивно проходится по файлам
    /// из текущей директории и её поддиректорий,
    /// применяя текущие шаблоны исключений для фильтрации.
    /// </summary>
    /// <param name="currentDirectory">Текущая директория.</param>
    private void ProcessDirectory(DirectoryInfo currentDirectory, string currentTabs)
    {
        CheckGitIngnore(currentDirectory);

        // Обработка поддиректорий
        foreach (var directory in currentDirectory.GetDirectories())
        {
            if (IsExcluded(directory.Name, true))
                continue;

            _fileMapCollector.AddDirectory(directory, currentTabs);

            ProcessDirectory(directory, currentTabs + TabSimbol);
        }

        // Обработка файлов в текущей директории
        foreach (var file in currentDirectory.GetFiles())
        {
            _fileMapCollector.AddFile(file, currentTabs);

            if (!IsExcluded(file.Name, false))
                _fileCollector.Add(file);
        }

        // Удаляем паттерны текущей директории из стека
        _excludePatternsStack.Pop();
    }

    /// <summary>
    /// Определяет, должен ли файл или директория быть исключены
    /// на основе текущего стека шаблонов исключений.
    /// </summary>
    /// <param name="name">Название файла или директории.</param>
    /// <param name="isDirectory">Указывает, является ли объект директорией.</param>
    /// <returns>True, если файл или директория должны быть исключены, иначе False.</returns>
    private bool IsExcluded(string name, bool isDirectory)
    {
        foreach (var patterns in _excludePatternsStack)
        {
            foreach (var regex in patterns)
            {
                if (regex.IsMatch(name))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Проверка существования .gitingore и добавление его правил в исключения.
    /// </summary>
    /// <param name="currentDirectory">Директория в которой проверяется наличие файла.</param>
    private void CheckGitIngnore(DirectoryInfo currentDirectory)
    {
        var gitIngnoreFile = currentDirectory.GetFiles(".gitignore").FirstOrDefault();
        _excludePatternsStack.Push(gitIngnoreFile.ParseGitignoreFile());
    }
}
