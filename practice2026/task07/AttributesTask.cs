using System;
using System.Reflection;

namespace task07
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class DisplayNameAttribute : Attribute
    {
        public string DisplayName { get; }

        public DisplayNameAttribute(string displayName) => DisplayName = displayName;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class VersionAttribute : Attribute
    {
        public int Major { get; }
        public int Minor { get; }

        public VersionAttribute(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }
    }

    [DisplayName("Пример класса")]
    [Version(1, 0)]
    public class SampleClass
    {
        [DisplayName("Числовое свойство")]
        public int Number { get; set; }

        [DisplayName("Тестовый метод")]
        public void TestMethod() { }
    }

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
}
