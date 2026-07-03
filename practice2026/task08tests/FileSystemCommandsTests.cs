using CommandRunner;
using FileSystemCommands;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace task08tests;

public class FileSystemCommandsTests : IDisposable
{
    private readonly string _testBaseDir;

    public FileSystemCommandsTests()
    {
        _testBaseDir = Path.Combine(Path.GetTempPath(), $"Task08Test_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testBaseDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testBaseDir))
        {
            Directory.Delete(_testBaseDir, true);
        }
    }

    [Fact]
    public void DirectorySizeCommand_CalculatesCorrectSize_InRootDirectory()
    {
        File.WriteAllText(Path.Combine(_testBaseDir, "file_a.txt"), "Hello");
        File.WriteAllText(Path.Combine(_testBaseDir, "file_b.txt"), "World");

        var command = new DirectorySizeCommand(_testBaseDir);
        command.Execute();

        Assert.Equal(10, command.TotalSizeBytes);
    }

    [Fact]
    public void FindFilesCommand_FiltersCorrectly_ByExtension()
    {
        File.WriteAllText(Path.Combine(_testBaseDir, "target.txt"), "Text");
        File.WriteAllText(Path.Combine(_testBaseDir, "ignore.log"), "Log");

        var command = new FindFilesCommand(_testBaseDir, "*.txt");
        command.Execute();

        Assert.Single(command.FoundFilePaths);
        Assert.Contains(command.FoundFilePaths, p => Path.GetFileName(p) == "target.txt");
    }

    [Fact]
    public void DirectorySizeCommand_CalculatesCorrectSize_WithNestedDirectories()
    {
        var subDir1 = Path.Combine(_testBaseDir, "Level1");
        var subDir2 = Path.Combine(subDir1, "Level2");
        Directory.CreateDirectory(subDir2);

        File.WriteAllText(Path.Combine(_testBaseDir, "root.doc"), "Hello World");
        File.WriteAllText(Path.Combine(subDir1, "sub1.txt"), "1234567890");
        File.WriteAllText(Path.Combine(subDir2, "sub2.doc"), "Document");
        File.WriteAllText(Path.Combine(subDir2, "sub3.txt"), "README");

        var command = new DirectorySizeCommand(_testBaseDir);
        command.Execute();

        Assert.Equal(35, command.TotalSizeBytes);
    }

    [Fact]
    public void FindFilesCommand_FindsFiles_AcrossNestedDirectories()
    {
        var subDir1 = Path.Combine(_testBaseDir, "Level1");
        var subDir2 = Path.Combine(subDir1, "Level2");
        Directory.CreateDirectory(subDir2);

        File.WriteAllText(Path.Combine(_testBaseDir, "root.doc"), "Hello World");
        File.WriteAllText(Path.Combine(subDir1, "sub1.txt"), "1234567890");
        File.WriteAllText(Path.Combine(subDir2, "sub2.doc"), "Document");
        File.WriteAllText(Path.Combine(subDir2, "sub3.txt"), "README");

        var command = new FindFilesCommand(_testBaseDir, "*.doc");
        command.Execute();

        Assert.Equal(2, command.FoundFilePaths.Count);
        Assert.Contains(command.FoundFilePaths, p => Path.GetFileName(p) == "root.doc");
        Assert.Contains(command.FoundFilePaths, p => Path.GetFileName(p) == "sub2.doc");
    }

    [Fact]
    public void Console_OutputValidation_MatchesExpectedFormat()
    {
        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        Program.Main();

        string result = consoleOutput.ToString();

        Assert.Contains("Общий размер: 43 байт", result);

        Assert.Contains("Найдено файлов: 2", result);
        Assert.Contains("data1.txt", result);
        Assert.Contains("data3.txt", result);
    }
}