using System.Text.RegularExpressions;
using SolutionToText.Models;

namespace SolutionToText.Services;

/// <summary>
/// Класс, рекурсивно собирающий файлы с указанными расширениями из директорий и их поддиректорий,
/// применяя фильтрацию на основе файлов .gitignore.
/// </summary>
class FileCollector
{
    /// <summary>
    /// Набор расширений файлов, которые необходимо включить в поиск.
    /// </summary>
    private HashSet<string> _includeExtensions = [".cs", ".js", ".css"];

    /// <summary>
    /// Стек списков предкомпилированных регулярных выражений для исключения файлов и директорий.
    /// </summary>
    private Stack<List<Regex>> _excludePatternsStack = new();

    /// <summary>
    /// Собирает файлы с расширениями из <see cref="_includeExtensions"/>, 
    /// начиная с указанной директории и её подпапок,
    /// применяя фильтрацию на основе шаблонов исключений из файлов .gitignore.
    /// </summary>
    /// <param name="rootPath">Путь к корневой директории, с которой начинается сбор файлов.</param>
    /// <returns>Список путей к найденным файлам.</returns>
    internal List<string> Collect(string rootPath)
    {
        var files = new List<string>();
        // Предварительное исключение папки "obj"
        var initialPatterns = new List<Regex>
        {
            new Regex(@"^obj$", RegexOptions.Compiled | RegexOptions.IgnoreCase)
        };
        _excludePatternsStack.Push(initialPatterns);
        CollectFilesRecursive(rootPath, files);
        _excludePatternsStack.Clear();
        return files;
    }

    /// <summary>
    /// Рекурсивно собирает файлы с соответствующими расширениями 
    /// из текущей директории и её поддиректорий,
    /// применяя текущие шаблоны исключений для фильтрации.
    /// </summary>
    /// <param name="currentDirectory">Текущая директория.</param>
    /// <param name="files">Список для накопления найденных файлов.</param>
    private void CollectFilesRecursive(DirectoryInfo currentDirectory, List<string> files)
    {
        // Проверяем наличие .gitignore в текущей директории
        var gitIngnoreFile = currentDirectory.GetFiles(".gitignore").FirstOrDefault();
        if (gitIngnoreFile != null)
        {
            var patterns = ParseGitignoreFile(gitIngnoreFile);
            _excludePatternsStack.Push(patterns);
        }
        else
        {
            _excludePatternsStack.Push(new());
        }

        // Обработка поддиректорий
        foreach (var dir in currentDirectory.GetDirectories())
        {
            // Применяем исключения к директории
            if (IsExcluded(dir.Name, true))
                continue;

            CollectFilesRecursive(dir, files);
        }

        // Обработка файлов в текущей директории
        foreach (var file in currentDirectory.GetFiles())
        {
            string fileName = Path.GetFileName(file);

            // Фильтрация по расширению и исключениям
            if (_includeExtensions.Contains(file.Extension) && !IsExcluded(fileName, false))
                files.Add(file);
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
    /// Читает файл .gitignore и возвращает список предкомпилированных регулярных выражений.
    /// </summary>
    /// <param name="gitIgnoreFile">Информация о файле .gitignore.</param>
    /// <returns>Список регулярных выражений для игнорирования.</returns>
    private List<Regex> ParseGitignoreFile(FileInfo gitIgnoreFile)
    {
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
            {
                regexPattern += @"(\\|/)?$";
            }

            patterns.Add(new Regex(regexPattern, options));
        }

        return patterns;
    }


    private string GetFilesStruct()
    {


        return ";";
    }
}
