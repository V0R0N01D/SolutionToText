namespace SolutionToText.Interfaces;

interface IContentWriter
{
    void WriteFilesStructure(IFileStructureCollector filesStructureCollector);

    void WriteFileContent(FileInfo file, char[] buffer, string rootPath);
}
