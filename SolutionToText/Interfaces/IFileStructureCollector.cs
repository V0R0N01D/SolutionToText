namespace SolutionToText.Interfaces;

interface IFileStructureCollector
{
    void AddFile(FileInfo file, string currentTab);

    void AddDirectory(DirectoryInfo directory, string currentTab);

    string GetFilesStructure();
}
