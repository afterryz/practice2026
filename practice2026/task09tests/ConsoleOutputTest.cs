using System;
using System.IO;
using Xunit;
using task09;

namespace task09tests;

public class ConsoleOutputTest
{
    private string ConsoleRun()
    {
        var output = new StringWriter();
        Console.SetOut(output);

        string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "FileSystemCommands", "bin", "Debug", "net8.0", "FileSystemCommands.dll"));
        string[] args = new string[] { path };

        Program.Main(args);
        return output.ToString();
    }

    [Fact]
    public void MetaDataOfDirectorySizeCommand()
    {
        string output = ConsoleRun();

        Assert.Contains("Библиотека: FileSystemCommands", output);
        Assert.Contains("Класс DirectorySizeCommand:", output);

        Assert.Contains("public String get_TargetDirectory()", output);
        Assert.Contains("public Int64 get_TotalSizeBytes()", output);
        Assert.Contains("private Void set_TotalSizeBytes(Int64 value)", output);
        Assert.Contains("public Void Execute()", output);

        Assert.Contains("DisplayNameAttribute", output);
        Assert.Contains("VersionAttribute", output);

        Assert.Contains("public DirectorySizeCommand(String targetDirectory)", output);
    }

    [Fact]
    public void MetaDataOfFindFilesCommand()
    {
        string output = ConsoleRun();

        Assert.Contains("Библиотека: FileSystemCommands", output);
        Assert.Contains("Класс FindFilesCommand:", output);

        Assert.Contains("public String get_TargetDirectory()", output);
        Assert.Contains("public String get_SearchPattern()", output);
        Assert.Contains("public List`1 get_FoundFilePaths()", output);
        Assert.Contains("private Void set_FoundFilePaths(List`1 value)", output);
        Assert.Contains("public Void Execute()", output);

        Assert.Contains("DisplayNameAttribute", output);
        Assert.Contains("VersionAttribute", output);

        Assert.Contains("public FindFilesCommand(String targetDirectory, String searchPattern)", output);
    }
}
