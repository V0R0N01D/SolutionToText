using System.Text.RegularExpressions;

namespace SolutionToText.Services;

/// <summary>
/// Класс для получения всех файлов рекурсивно с поддержкой .gitignore
/// </summary>
internal class FileCollector
{
	private HashSet<string> _includeExtensions = [".cs"];
	private Stack<List<string>> _excludePatternsStack = new();

	internal List<string> Collect(string rootPath)
	{
		var files = new List<string>();
		_excludePatternsStack.Push(["obj/"]); // Начинаем без папки obj
		CollectFilesRecursive(rootPath, files);
		_excludePatternsStack.Clear(); // Очищаем стек после обхода
		return files;
	}

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
			// Если .gitignore нет, добавляем пустой список
			_excludePatternsStack.Push(new());
		}

		// Обрабатываем поддиректории
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
	/// Чтение файла .gitignore
	/// </summary>
	/// <param name="gitignorePath">Путь до файла</param>
	/// <returns>Список паттернов для игнорирования</returns>
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
