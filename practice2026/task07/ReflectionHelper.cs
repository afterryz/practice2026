using System;
using System.Reflection;

namespace task07;

public static class ReflectionHelper
{
    public static void PrintTypeInfo(Type type)
    {
        if (type.GetCustomAttribute<DisplayNameAttribute>() is { } classDisplay)
        {
            Console.WriteLine($"[{type.Name}] - {classDisplay.DisplayName}");
        }

        if (type.GetCustomAttribute<VersionAttribute>() is { } classVersion)
        {
            Console.WriteLine($"Версия: {classVersion.Major}.{classVersion.Minor}");
        }

        Console.WriteLine("\n--- Методы ---");
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
        bool hasMethods = false;

        foreach (var method in methods)
        {
            if (method.GetCustomAttribute<DisplayNameAttribute>() is { } methodAttr)
            {
                Console.WriteLine($"{method.Name}() -> {methodAttr.DisplayName}");
                hasMethods = true;
            }
        }
        if (!hasMethods) Console.WriteLine("Методов с атрибутами не найдено.");

        Console.WriteLine("\n--- Свойства ---");
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
        bool hasProps = false;

        foreach (var prop in properties)
        {
            if (prop.GetCustomAttribute<DisplayNameAttribute>() is { } propAttr)
            {
                Console.WriteLine($"{prop.Name} (тип {prop.PropertyType.Name}) -> {propAttr.DisplayName}");
                hasProps = true;
            }
        }
        if (!hasProps) Console.WriteLine("Свойств с атрибутами не найдено.");
    }
}
