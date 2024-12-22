namespace SolutionToText.Interfaces;

interface ISourceFileCollector
{
    void AddFileSource(FileInfo file);

    List<FileInfo> GetSourceFiles();
}
