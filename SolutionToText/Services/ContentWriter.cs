using SolutionToText.Interfaces;

namespace SolutionToText.Services;

/// <summary>
/// Provides methods for writing file structure and file contents to an output file.
/// </summary>
internal sealed class ContentWriter : IContentWriter, IDisposable
{
    private readonly StreamWriter _writer;
    private bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentWriter"/> class with the specified output file path.
    /// </summary>
    /// <param name="filePath">The path to the output file.</param>
    internal ContentWriter(string filePath)
    {
        _writer = new StreamWriter(filePath);
    }

    /// <inheritdoc />
    public void WriteFilesStructure(IFileStructureCollector filesStructureCollector)
    {
        _writer.WriteLine(filesStructureCollector.GetFilesStructure());
    }

    /// <inheritdoc />
    public void WriteFileContent(FileInfo file, char[] buffer, string rootPath)
    {
        _writer.WriteLine($"File content {file.FullName.Replace(rootPath, string.Empty)}:");
        CopyFileContent(buffer, file);
        _writer.WriteLine(_writer.NewLine);
    }

    /// <summary>
    /// Copies the content of the source file to the destination stream,
    /// using the provided buffer.
    /// </summary>
    /// <param name="buffer">The buffer used for copying.</param>
    /// <param name="sourceFile">Source file information</param>
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

    private void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
            _writer.Dispose();

        _disposed = true;
    }

    ~ContentWriter() => Dispose(false);

    #endregion
}