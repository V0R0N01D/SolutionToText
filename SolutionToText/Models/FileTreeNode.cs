namespace SolutionToText.Models;

internal struct FileTreeNode
{
    internal FileTreeNode(string currentFolderTitle)
    {
        CurrentFolderTitle = currentFolderTitle;
    }

    internal string CurrentFolderTitle { get; set; }

    internal List<string> FileTitles { get; set; } = new();

    internal List<FileTreeNode>? Folders { get; set; }
}
