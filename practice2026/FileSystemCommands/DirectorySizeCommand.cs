using CommandLib;
using System.IO;
using System.Linq;
using task07;

namespace FileSystemCommands;

[DisplayName("Directory Size Command")]
[Version(1, 0)]
public class DirectorySizeCommand : CommandLib.ICommand
{
    public string TargetDirectory { get; }
    public long TotalSizeBytes { get; private set; }

    public DirectorySizeCommand(string targetDirectory)
    {
        TargetDirectory = targetDirectory;
    }

    public void Execute()
    {
        TotalSizeBytes = 0;

        if (!Directory.Exists(TargetDirectory))
            return;

        var directoryInfo = new DirectoryInfo(TargetDirectory);

        TotalSizeBytes = directoryInfo
            .EnumerateFiles("*", SearchOption.AllDirectories)
            .Sum(file => file.Length);
    }
}
