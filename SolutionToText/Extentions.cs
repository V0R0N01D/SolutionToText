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
		string sourceFilePath,
		StreamWriter destinationWriter)
	{
		using var sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read);
		using var reader = new StreamReader(sourceStream);

		var charsRead = 0;

		while ((charsRead = reader.Read(buffer, 0, buffer.Length)) > 0)
		{
			destinationWriter.Write(buffer, 0, charsRead);
		}
	}
}
