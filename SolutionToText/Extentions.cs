using System.Text.RegularExpressions;

namespace SolutionToText;

/// <summary>
/// Класс расширений для удобства работы с потоками и буферами.
/// </summary>
static class Extensions
{
	/// <summary>
	/// Копирует содержимое исходного файла в поток назначения, 
	/// используя переданный буфер для чтения данных порциями.
	/// </summary>
	/// <param name="buffer">Буфер, используемый для копирования.</param>
	/// <param name="sourceFilePath">Путь к исходному файлу.</param>
	/// <param name="destinationWriter">Поток записи для записи содержимого файла.</param>
	internal static void CopyFileContent(
		this char[] buffer,
		FileInfo sourceFilePath,
		StreamWriter destinationWriter)
	{
#warning мб заменить на метод из fileinfo
        using var sourceStream = new FileStream(sourceFilePath.FullName,
            FileMode.Open, FileAccess.Read);
		using var reader = new StreamReader(sourceStream);

		var charsRead = 0;

		while ((charsRead = reader.Read(buffer, 0, buffer.Length)) > 0)
		{
			destinationWriter.Write(buffer, 0, charsRead);
		}
	}

    /// <summary>
    /// Читает файл .gitignore и возвращает список предкомпилированных регулярных выражений.
    /// </summary>
    /// <param name="gitIgnoreFile">Информация о файле .gitignore.</param>
    /// <returns>Список регулярных выражений для игнорирования.</returns>
    internal static List<Regex> ParseGitignoreFile(this FileInfo? gitIgnoreFile)
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
            {
                regexPattern += @"(\\|/)?$";
            }

            patterns.Add(new Regex(regexPattern, options));
        }

        return patterns;
    }
}
