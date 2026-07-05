using System;
using System.Linq;
using System.Reflection;

namespace task09;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        Assembly metaData = Assembly.LoadFrom(args[0]);
        Console.WriteLine($"Библиотека: {metaData.GetName().Name}");

        Type[] types = metaData.GetTypes();
        foreach (Type type in types)
        {
            if (!type.IsClass)
            {
                continue;
            }

            Console.WriteLine($"Класс {type.Name}:");
            Console.WriteLine("Методы:");

            MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MethodInfo method in methods)
            {
                string modificator = method.IsPublic ? "public" : "private";
                string typeMethod = method.ReturnType.Name;
                string nameMethod = method.Name;
                ParameterInfo[] parameters = method.GetParameters();
                string parameterStr = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));

                Console.WriteLine($"{modificator} {typeMethod} {nameMethod}({parameterStr})");
            }

            Console.WriteLine("Атрибуты:");
            object[] attributes = type.GetCustomAttributes(false);
            foreach (object attribute in attributes)
            {
                Type attr = attribute.GetType();
                Console.WriteLine($"{attr.Name}");
            }

            Console.WriteLine("Конструкторы:");
            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (ConstructorInfo constr in constructors)
            {
                string modificator = constr.IsPublic ? "public" : "private";
                ParameterInfo[] parameters = constr.GetParameters();
                string parameterStr = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));

                Console.WriteLine($"{modificator} {type.Name}({parameterStr})");
            }
        }
    }
}
