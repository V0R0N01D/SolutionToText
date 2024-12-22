using SolutionToText.Interfaces;

namespace SolutionToText.Services;

class ContentWriter : IContentWriter, IDisposable
{
    private readonly StreamWriter _writer;
    private bool _disposed = false;

    internal ContentWriter(string filePath)
    {
        _writer = new StreamWriter(filePath);
    }

    public void WriteFilesStructure(IFileStructureCollector filesStructureCollector)
    {
        _writer.WriteLine(filesStructureCollector.GetFilesStructure());
    }

    public void WriteFileContent(FileInfo file, char[] buffer, string rootPath)
    {
        _writer.WriteLine($"Содержимое файла {file.FullName.Replace(rootPath, string.Empty)}:");
        CopyFileContent(buffer, file);
        _writer.WriteLine(_writer.NewLine);
    }

    /// <summary>
    /// Копирует содержимое исходного файла в поток назначения, 
    /// используя переданный буфер для чтения данных порциями.
    /// </summary>
    /// <param name="buffer">Буфер, используемый для копирования.</param>
    /// <param name="sourceFile">Путь к исходному файлу.</param>
    private void CopyFileContent(
        char[] buffer,
        FileInfo sourceFile)
    {
        using var reader = sourceFile.OpenText();

        var charsRead = 0;
        while ((charsRead = reader.Read(buffer, 0, buffer.Length)) > 0)
        {
            _writer.Write(buffer, 0, charsRead);
        }
    }

    #region Resource cleanup

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
            _writer.Dispose();
    }

    ~ContentWriter() => Dispose(false);

    #endregion
}


