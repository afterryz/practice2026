using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using CommandLib;

namespace CommandRunner;

public class Program
{
    public static void Main()
    {
        string tempDirPath = Path.Combine(Path.GetTempPath(), "Task08_DemoDir");
        Directory.CreateDirectory(tempDirPath);
        File.WriteAllText(Path.Combine(tempDirPath, "data1.txt"), "Hello FIIT");
        File.WriteAllText(Path.Combine(tempDirPath, "data2.log"), "Some log information");
        File.WriteAllText(Path.Combine(tempDirPath, "data3.txt"), "C# is awesome");

        try
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dllPath = Path.Combine(baseDir, "FileSystemCommands.dll");

            if (!File.Exists(dllPath))
            {
                Console.WriteLine("Ошибка: не удалось найти FileSystemCommands.dll в папке приложения.");
                return;
            }

            Assembly loadedAssembly = Assembly.LoadFrom(dllPath);

            Type sizeCommandType = loadedAssembly.GetType("FileSystemCommands.DirectorySizeCommand")!;
            if (sizeCommandType != null)
            {
                CommandLib.ICommand sizeCommand = (CommandLib.ICommand)Activator.CreateInstance(sizeCommandType, tempDirPath)!;
                sizeCommand.Execute();

                PropertyInfo sizeProp = sizeCommandType.GetProperty("TotalSizeBytes")!;
                long totalSize = (long)sizeProp.GetValue(sizeCommand)!;

                Console.WriteLine($"[Информация о каталоге]\nПуть: {tempDirPath}");
                Console.WriteLine($"Общий размер: {totalSize} байт.\n");
            }

            Type findCommandType = loadedAssembly.GetType("FileSystemCommands.FindFilesCommand")!;
            if (findCommandType != null)
            {
                CommandLib.ICommand findCommand = (CommandLib.ICommand)Activator.CreateInstance(findCommandType, tempDirPath, "*.txt")!;
                findCommand.Execute();

                PropertyInfo filesProp = findCommandType.GetProperty("FoundFilePaths")!;
                var matchedFiles = (List<string>)filesProp.GetValue(findCommand)!;

                Console.WriteLine($"[Результаты поиска (*.txt)]\nНайдено файлов: {matchedFiles.Count}");
                foreach (var filePath in matchedFiles)
                {
                    Console.WriteLine($" - {Path.GetFileName(filePath)}");
                }
            }
        }
        finally
        {
            if (Directory.Exists(tempDirPath))
            {
                Directory.Delete(tempDirPath, true);
            }
        }
    }
}
