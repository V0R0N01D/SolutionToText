using SolutionToText.Interfaces;

namespace SolutionToText.Services;

/// <summary>
/// Provides methods for writing file structure and file contents to an output file.
/// </summary>
internal sealed class ContentWriter : IContentWriter, IDisposable
{
    private readonly StreamWriter _writer;
    private readonly char[] _buffer = new char[2048];
    private bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentWriter"/> class
    /// with the specified output file path.
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
    public void WriteFileContent(FileInfo file, string rootPath)
    {
        _writer.WriteLine($"File content {file.FullName.Replace(rootPath, string.Empty)}:");
        CopyFileContent(file);
        _writer.WriteLine(_writer.NewLine);
    }

    /// <summary>
    /// Copies the content of the source file to the destination stream,
    /// using the provided buffer.
    /// </summary>
    /// <param name="sourceFile">Source file information.</param>
    private void CopyFileContent(FileInfo sourceFile)
    {
        using var reader = sourceFile.OpenText();

        var charsRead = 0;
        while ((charsRead = reader.Read(_buffer, 0, _buffer.Length)) > 0)
        {
            _writer.Write(_buffer, 0, charsRead);
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