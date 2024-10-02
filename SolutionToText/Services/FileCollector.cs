using System.Text.RegularExpressions;

namespace SolutionToText.Services;

/// <summary>
/// Класс для рекурсивного поиска файлов в директориях с поддержкой фильтрации на основе файла .gitignore.
/// </summary>
class FileCollector
{
	private HashSet<string> _includeExtensions = [".cs"];
	private Stack<List<string>> _excludePatternsStack = new();

	/// <summary>
	/// Собирает файлы, соответствующие включенным расширениям, 
	/// начиная с указанной директории и всех её подпапок, 
	/// с применением фильтрации на основе шаблонов исключений (.gitignore).
	/// </summary>
	/// <param name="rootPath"></param>
	/// <returns></returns>
	internal List<string> Collect(string rootPath)
	{
		var files = new List<string>();
		_excludePatternsStack.Push(["obj/"]); // Начинаем без папки obj
		CollectFilesRecursive(rootPath, files);
		_excludePatternsStack.Clear(); // Очищаем стек после обхода
		return files;
	}

	/// <summary>
	/// Рекурсивно собирает файлы из текущей директории и её подпапок, 
	/// применяя шаблоны исключений для фильтрации.
	/// </summary>
	/// <param name="currentPath">Текущий путь.</param>
	/// <param name="files">Список файлов.</param>
	private void CollectFilesRecursive(string currentPath, List<string> files)
	{
		// Проверяем наличие .gitignore в текущей директории
		string gitignorePath = Path.Combine(currentPath, ".gitignore");
		if (File.Exists(gitignorePath))
		{
			var patterns = ParseGitignoreFile(gitignorePath);
			_excludePatternsStack.Push(patterns);
		}
		else
		{
			_excludePatternsStack.Push(new());
		}

		// Обработка поддиректорий
		foreach (var dir in Directory.GetDirectories(currentPath))
		{
			string dirName = Path.GetFileName(dir);

			// Применяем исключения к директории
			if (IsExcluded(dirName, true))
				continue;

			CollectFilesRecursive(dir, files);
		}

		// Обрабатываем файлы в текущей директории
		foreach (var file in Directory.GetFiles(currentPath))
		{
			string fileName = Path.GetFileName(file);

			// Применяем фильтрацию по расширению и исключениям
			if (_includeExtensions.Contains(Path.GetExtension(file)) && !IsExcluded(fileName, false))
				files.Add(file);
		}

		// Удаляем паттерны текущей директории из стека
		_excludePatternsStack.Pop();
	}

	/// <summary>
	/// Определяет, должен ли файл или директория быть исключены 
	/// на основе текущего стека шаблонов исключений.
	/// </summary>
	/// <param name="name">Название файла (с расширением) или директории.</param>
	/// <param name="isDirectory">Является ли параметр name директорией.</param>
	/// <returns>Результат проверки на исключения (true - файл исключается, false - файл остается).</returns>
	private bool IsExcluded(string name, bool isDirectory)
	{
		foreach (var patterns in _excludePatternsStack)
		{
			foreach (var pattern in patterns)
			{
				if (PatternMatches(name, pattern, isDirectory))
					return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Проверяет, соответствует ли имя файла или директории шаблону, 
	/// с учётом того, является ли объект директорией.
	/// </summary>
	/// <param name="name">Название файла (с расширением) или директории.</param>
	/// <param name="pattern">Шаблон.</param>
	/// <param name="isDirectory">Является ли параметр name директорией.</param>
	/// <returns>Подходит ли файл или директория под паттерн.</returns>
	private bool PatternMatches(string name, string pattern, bool isDirectory)
	{
		// Упрощенная обработка паттернов из .gitignore
		// Для полной поддержки стоит использовать библиотеку
		// Но для простоты заменим '*' на '.*' и '?' на '.'

		// Обработка паттернов директорий
		if (pattern.EndsWith("/"))
		{
			if (!isDirectory)
				return false;

			pattern = pattern.TrimEnd('/');
		}

		// Конвертируем паттерн в регулярное выражение
		string regexPattern = "^" + Regex.Escape(pattern)
								.Replace("\\*", ".*")
								.Replace("\\?", ".") + "$";

		return Regex.IsMatch(name, regexPattern, RegexOptions.IgnoreCase);
	}

	/// <summary>
	/// Читает файл .gitignore и возвращает список шаблонов исключений.
	/// </summary>
	/// <param name="gitignorePath">Путь до файла.</param>
	/// <returns>Список паттернов для игнорирования.</returns>
	private List<string> ParseGitignoreFile(string gitignorePath)
	{
		var patterns = new List<string>();
		var lines = File.ReadAllLines(gitignorePath);

		foreach (var line in lines)
		{
			var trimmedLine = line.Trim();

			if (string.IsNullOrEmpty(trimmedLine))
				continue;

			// Игнорирование комментариев
			if (trimmedLine.StartsWith("#"))
				continue;

			// Игнорирование отрицательных паттернов (начинающиеся с '!')
			if (trimmedLine.StartsWith("!"))
				continue;

			patterns.Add(trimmedLine);
		}

		return patterns;
	}
}
