using System.Text.RegularExpressions;
using SolutionToText.Interfaces;

namespace SolutionToText.Services;

/// <summary>
/// Класс который рекурсивно проходится по директориям и передает данные в другие классы.
/// </summary>
class DirectoryWalker : IDirectoryWalker
{
    private const char TabSymbol = '-';

    private readonly IFileStructureCollector _fileMapCollector;
    private readonly ISourceFileCollector _fileCollector;

    /// <summary>
    /// Стек состоящий из списков предкомпилированных регулярных выражений
    /// для исключения файлов и директорий.
    /// </summary>
    readonly Stack<List<Regex>> _excludePatternsStack = new();

    internal DirectoryWalker(IFileStructureCollector fileMapCollector,
        ISourceFileCollector fileCollector)
    {
        _fileCollector = fileCollector;
        _fileMapCollector = fileMapCollector;

        // Предварительное исключение папок "obj", ".git" и "bin"
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
    public void WalkDirectory(DirectoryInfo rootPath)
    {
        ProcessDirectory(rootPath, TabSymbol.ToString());
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

            ProcessDirectory(directory, currentTabs + TabSymbol);
        }

        // Обработка файлов в текущей директории
        foreach (var file in currentDirectory.GetFiles())
        {
            _fileMapCollector.AddFile(file, currentTabs);

            if (!IsExcluded(file.Name, false))
                _fileCollector.AddFileSource(file);
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
        _excludePatternsStack.Push(ParseGitignoreFile(gitIngnoreFile));
    }

    /// <summary>
    /// Читает файл .gitignore и возвращает список предкомпилированных регулярных выражений.
    /// </summary>
    /// <param name="gitIgnoreFile">Информация о файле .gitignore.</param>
    /// <returns>Список регулярных выражений для игнорирования.</returns>
    private List<Regex> ParseGitignoreFile(FileInfo? gitIgnoreFile)
    {
        if (gitIgnoreFile == null || !gitIgnoreFile.Exists)
            return new();

        var patterns = new List<Regex>();
        var lines = File.ReadAllLines(gitIgnoreFile.FullName);

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrEmpty(trimmedLine))
                continue;

            // Игнорирование комментариев
            if (trimmedLine.StartsWith("#"))
                continue;

            // Игнорирование отрицательных паттернов (начинающихся с '!')
            if (trimmedLine.StartsWith("!"))
                continue;

            // Обработка паттернов директорий
            bool isDirectoryPattern = trimmedLine.EndsWith("/");

            string pattern = trimmedLine.TrimEnd('/');

            // Конвертируем паттерн в регулярное выражение
            string regexPattern = "^" + Regex.Escape(pattern)
                                        .Replace("\\*", ".*")
                                        .Replace("\\?", ".") + "$";

            var options = RegexOptions.Compiled | RegexOptions.IgnoreCase;

            // Если это паттерн для директории, добавляем проверку на конец строки
            if (isDirectoryPattern)
                regexPattern += @"(\\|/)?$";

            patterns.Add(new Regex(regexPattern, options));
        }

        return patterns;
    }
}
