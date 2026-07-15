using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLib;
using task07;

namespace FileSystemCommands;

[DisplayName("Find Files Command")]
[Version(1, 1)]
public class FindFilesCommand : CommandLib.ICommand
{
    public string TargetDirectory { get; }
    public string SearchPattern { get; }
    public List<string> FoundFilePaths { get; private set; }

    public FindFilesCommand(string targetDirectory, string searchPattern)
    {
        TargetDirectory = targetDirectory;
        SearchPattern = searchPattern;
        FoundFilePaths = new List<string>();
    }

    public void Execute()
    {
        FoundFilePaths.Clear();

        if (!Directory.Exists(TargetDirectory))
            return;

        var directoryInfo = new DirectoryInfo(TargetDirectory);

        FoundFilePaths = directoryInfo
            .EnumerateFiles(SearchPattern, SearchOption.AllDirectories)
            .Select(file => file.FullName)
            .ToList();
    }
}
